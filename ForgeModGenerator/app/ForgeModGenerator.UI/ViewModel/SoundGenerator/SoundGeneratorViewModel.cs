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
        public ICommand ChangeSoundCommand => changeSoundCommand ?? (changeSoundCommand = new RelayCommand<Tuple<SoundEvent, Sound>>(ChangeSoundPath));

        private bool CanChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            string newFileName = null;
            try
            {
                string modname = Mod.GetModnameFromPath(param.Item2.FilePath);
                string modid = Mod.GetModidFromPath(param.Item2.Name);
                string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
                string oldFilePath = param.Item2.FilePath.Replace("\\", "/");
                string extension = Path.GetExtension(oldFilePath);
                newFileName = param.Item2.Name.Remove(0, param.Item2.Name.IndexOf(":") + 1);
                string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newFileName)}{extension}";
                string newFilePath = null;
                try
                {
                    newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");
                }
                catch (Exception pathEx)
                {
                    Log.Error(pathEx, $"Path is not valid {newFilePathToValidate}", true);
                    return false;
                }
                if (oldFilePath != newFilePath)
                {
                    if (param.Item1.Sounds.Any(x => x.FilePath == newFilePath))
                    {
                        Log.Warning($"{param.Item1.EventName} already has {newFileName} sound", true);
                        return false;
                    }
                    if (!File.Exists(newFilePath))
                    {
                        new FileInfo(newFilePath).Directory.Create();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Can't change sound name to {newFileName}", true);
                return false;
            }
            return true;
        }

        private void ChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            try
            {
                string modname = Mod.GetModnameFromPath(param.Item2.FilePath);
                string modid = Mod.GetModidFromPath(param.Item2.Name);
                string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
                string oldFilePath = param.Item2.FilePath.Replace("\\", "/");
                string extension = Path.GetExtension(oldFilePath);
                string newFileName = param.Item2.Name.Remove(0, param.Item2.Name.IndexOf(":") + 1);
                string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newFileName)}{extension}";
                string newFilePath = null;
                try
                {
                    newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");
                }
                catch (Exception pathEx)
                {
                    Log.Error(pathEx, $"Path is not valid {newFilePathToValidate}", true);
                    return;
                }

                if (oldFilePath != newFilePath)
                {
                    try
                    {
                        if (param.Item1.Sounds.Any(x => x.FilePath == newFilePath))
                        {
                            Log.Warning($"{param.Item1.EventName} already has this sound", true);
                            return;
                        }
                        else if (!File.Exists(newFilePath))
                        {
                            new FileInfo(newFilePath).Directory.Create();
                            File.Move(oldFilePath, newFilePath);
                        }
                        else if (ReferenceCounter.GetReferenceCount(oldFilePath) > 1)
                        {
                            FileSystem.DeleteFile(oldFilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        param.Item2.SetFileItem(newFilePath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't change {param.Item2.FileName} to {newFileName}. Make sure the file is not opened by any process", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while changing sound name", true);
            }
        }

        private void DeleteSound(Tuple<SoundEvent, Sound> param)
        {
            DeleteSound(param, false);
        }

        private void DeleteSound(Tuple<SoundEvent, Sound> param, bool forceDeleteAll)
        {
            try
            {
                if (!forceDeleteAll && param.Item1.Sounds.Count == 1)
                {
                    Log.Warning("SoundEvent must have at least 1 sound", true);
                    return;
                }
                if (param.Item1.Sounds.Remove(param.Item2))
                {
                    if (ReferenceCounter.IsReferenced(param.Item2.FilePath))
                    {
                        return; // do not delete file since it's referenced by any other sound
                    }
                    try
                    {
                        FileSystem.DeleteFile(param.Item2.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't delete {param.Item2.FilePath}. Make sure it's not used by any process", true);
                        param.Item1.Sounds.Add(param.Item2); // delete failed, so get item back to collection
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
                            else if (soundEvent.Sounds.Any(x => x.FilePath == newFilePath))
                            {
                                Log.Warning($"{soundEvent.EventName} already has this sound", true);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Couldn't add {fileName} to {soundEvent.EventName}. Make sure the file is not opened by any process", true);
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

        protected override bool CanCloseEditForm(bool result, IFileItem fileBeforeEdit, IFileItem fileAfterEdit)
        {
            if (result)
            {
                SoundEvent cachedSoundEvent = (SoundEvent)fileBeforeEdit;
                SoundEvent newSoundEvent = (SoundEvent)fileAfterEdit;
                if (ReferenceCounter.GetReferenceCount(newSoundEvent.EventName) > 1)
                {
                    Log.Warning("The sound event name already exists. Duplicates are not allowed", true);
                    return false;
                }
                return !newSoundEvent.Sounds.Any(sound => !CanChangeSoundPath(new Tuple<SoundEvent, Sound>(newSoundEvent, sound)));
            }
            else
            {
                if (fileAfterEdit.IsDirty)
                {
                    DialogResult msgResult = MessageBox.Show("Are you sure you want to exit form? Changes won't be saved", "Unsaved changes", MessageBoxButtons.YesNo);
                    if (msgResult == DialogResult.No)
                    {
                        return false;
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
                foreach (Sound sound in soundEvent.Sounds)
                {
                    ChangeSoundPath(new Tuple<SoundEvent, Sound>(soundEvent, sound));
                }
                ForceJsonUpdate(); // temporary solution
            }
            else
            {
                // TODO: Undo commands
                base.OnEdited(result, file);
            }
            soundEvent.IsDirty = false;
        }

        protected override void Remove(Tuple<IFileFolder, IFileItem> param)
        {
            try
            {
                if (param.Item1.RemoveFile(param.Item2))
                {
                    SoundEvent soundEvent = (SoundEvent)param.Item2;
                    //foreach (Sound sound in soundEvent.Sounds)
                    //{
                    //    DeleteSound(new Tuple<SoundEvent, Sound>(soundEvent, sound), true);
                    //}
                    int length = soundEvent.Sounds.Count;
                    for (int i = 0; i < length; i++)
                    {
                        DeleteSound(new Tuple<SoundEvent, Sound>(soundEvent, soundEvent.Sounds[i]), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while trying to remove file", true);
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
