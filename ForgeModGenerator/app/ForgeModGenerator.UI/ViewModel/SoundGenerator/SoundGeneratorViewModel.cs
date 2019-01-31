using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FileListViewModelBase<SoundEvent>
    {
        public override string CollectionRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public SoundsGeneratorPreferences Preferences { get; }

        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedExtensions = new string[] { ".ogg" };
            FileEditForm = new UserControls.SoundEditForm() {
                AddSoundCommand = AddSoundCommand,
                DeleteSoundCommand = DeleteSoundCommand,
                ChangeSoundCommand = ChangeSoundCommand
            };
            Preferences = sessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
            Refresh();
            OnFileAdded += AddSoundToJson;
            OnFileRemoved += RemoveSoundFromJson;
        }

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(ForceJsonUpdate));

        private ICommand addSoundCommand;
        public ICommand AddSoundCommand => addSoundCommand ?? (addSoundCommand = new RelayCommand<SoundEvent>(AddSound));

        private ICommand deleteSoundCommand;
        public ICommand DeleteSoundCommand => deleteSoundCommand ?? (deleteSoundCommand = new RelayCommand<Tuple<SoundEvent, Sound>>(DeleteSound));

        private ICommand changeSoundCommand;
        public ICommand ChangeSoundCommand => changeSoundCommand ?? (changeSoundCommand = new RelayCommand<Sound>(ChangeSoundName));

        private void ChangeSoundName(Sound obj)
        {
            throw new NotImplementedException();
        }

        private void DeleteSound(Tuple<SoundEvent, Sound> param)
        {
            try
            {
                if (param.Item1.Sounds.Count == 1)
                {
                    Log.Warning("SoundEvent must have at least 1 sound", true);
                    return;
                }
                if (param.Item1.Sounds.Remove(param.Item2))
                {
                    try
                    {
                        FileSystem.DeleteFile(param.Item2.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't delete {param.Item2.FilePath}. Make sure it's not used by any process", true);
                        param.Item1.Sounds.Add(param.Item2); // delete failed, so get item back to collection
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while trying to remove sound", true);
            }
        }

        private void AddSound(SoundEvent soundEvent)
        {
            try
            {
                DialogResult result = OpenFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (string filePath in OpenFileDialog.FileNames)
                    {
                        string fileName = new FileInfo(filePath).Name;
                        string newFilePath = Path.Combine(soundEvent.FilePath, fileName);
                        try
                        {
                            if (!File.Exists(newFilePath))
                            {
                                File.Copy(filePath, newFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Couldn't add {fileName} to {soundEvent.EventName}. Make sure the file is not opened by any process.", true);
                            continue;
                        }
                        Sound newSound = new Sound(SessionContext.SelectedMod.ModInfo.Modid, newFilePath);
                        soundEvent.Sounds.Add(newSound);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while adding new sound", true);
            }
        }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override bool Refresh()
        {
            bool canRefresh = base.Refresh();
            ShouldUpdate = canRefresh ? IsUpdateAvailable() : false;
            return canRefresh;
        }

        protected override void FileCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.FileCollection_CollectionChanged(sender, e);
            ShouldUpdate = CanRefresh() ? IsUpdateAvailable() : false;
        }

        protected override bool CanBeEdited(bool result, IFileItem file)
        {
            if (result)
            {
                SoundEvent soundEvent = (SoundEvent)file;
                int foundOccurencies = 0;
                foreach (FileList<SoundEvent> fileList in Files)
                {
                    foreach (SoundEvent item in fileList)
                    {
                        if (item.EventName == soundEvent.EventName)
                        {
                            foundOccurencies++;
                            if (foundOccurencies > 1)
                            {
                                Log.Warning("The sound event name already exists. Duplicates are not allowed", true);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        protected override void OnEdited(bool result, IFileItem file)
        {
            SoundEvent soundEvent = (SoundEvent)file;
            if (result)
            {
                // TODO: Save changes
                ForceJsonUpdate(); // temporary solution
            }
            else
            {
                // undo commands
                base.OnEdited(result, file);
            }
        }

        protected override ObservableCollection<FileList<SoundEvent>> FindCollection(string path, bool createRootIfEmpty)
        {
            try
            {
                string soundsFolder = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                ObservableCollection<FileList<SoundEvent>> rootCollection = null;
                if (!File.Exists(path))
                {
                    File.AppendAllText(path, "{}");
                    return createRootIfEmpty ? CreateEmptyRoot(soundsFolder) : rootCollection;
                }
                Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

                rootCollection = JsonConvert.DeserializeObject<ObservableCollection<FileList<SoundEvent>>>(File.ReadAllText(path), converter);
                if (createRootIfEmpty && (rootCollection == null || rootCollection.Count <= 0))
                {
                    rootCollection = CreateEmptyRoot(soundsFolder);
                }
                return rootCollection;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't load sounds.json. Should never happened. Report a bug");
                return null;
            }
        }

        protected void AddSoundToJson(object item)
        {
            if (item is SoundEvent soundEvent)
            {
                if (!HasSoundWritten(soundEvent.EventName))
                {
                    // TODO: Add sound
                }
            }
            else if (item is Sound sound)
            {
                if (!HasSoundWritten(sound.Name))
                {
                    // TODO: Add sound
                }
            }
            ForceJsonUpdate(); // temporary solution
        }

        protected void RemoveSoundFromJson(object item)
        {
            if (item is SoundEvent soundEvent)
            {
                if (HasSoundWritten(soundEvent.EventName))
                {
                    foreach (Sound sound in soundEvent.Sounds)
                    {
                        // TODO: Remove sound
                    }
                }
            }
            else if (item is Sound sound)
            {
                if (HasSoundWritten(sound.Name))
                {
                    // TODO: Remove sound
                }
            }
            ForceJsonUpdate(); // temporary solution
        }

        protected void ForceJsonUpdate()
        {
            try
            {
                Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                string json = JsonConvert.SerializeObject(Files, Preferences.JsonFormatting, converter);
                File.WriteAllText(CollectionRootPath, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't update json, this should not happened. Report a bug", true);
            }
        }

        protected bool HasSoundWritten(string sound) => File.ReadLines(CollectionRootPath).Any(x => x.Contains($"\"{sound}\""));

        protected bool IsUpdateAvailable()
        {
            try
            {
                Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                string newJson = JsonConvert.SerializeObject(Files, Preferences.JsonFormatting);
                string savedJson = File.ReadAllText(CollectionRootPath);
                if (newJson == savedJson)
                {
                    newJson = JsonConvert.SerializeObject(Files, Preferences.JsonFormatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
                }
                return newJson == savedJson;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return false;
            }
        }
    }
}
