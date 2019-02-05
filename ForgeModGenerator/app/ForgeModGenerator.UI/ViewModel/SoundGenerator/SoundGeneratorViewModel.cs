using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FolderListViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedFileExtensions = new string[] { ".ogg" };
            FileEditForm = new UserControls.SoundEditForm();
            Preferences = sessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
            Refresh();
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public SoundsGeneratorPreferences Preferences { get; }

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(ForceJsonUpdate));

        protected override void AddNewFolder()
        {
            try
            {
                OpenFolderDialog.SelectedPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                DialogResult dialogResult = OpenFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string newfolderPath = OpenFolderDialog.SelectedPath;
                    SoundEvent newFolder = SoundEvent.CreateEmpty(newfolderPath);
                    SetDelegates(newFolder);
                    AddNewFileToFolder(newFolder);
                    Folders.Add(newFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        private bool CanChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            string newFileName = null;
            try
            {
                string modname = Mod.GetModnameFromPath(param.Item2.Info.FullName);
                string modid = Mod.GetModidFromPath(param.Item2.Name);
                string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
                string oldFilePath = param.Item2.Info.FullName.Replace("\\", "/");
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
                    if (param.Item1.Files.Any(x => x.Info.FullName == newFilePath))
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
                string modname = Mod.GetModnameFromPath(param.Item2.Info.FullName);
                string modid = Mod.GetModidFromPath(param.Item2.Name);
                string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
                string oldFilePath = param.Item2.Info.FullName.Replace("\\", "/");
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
                        if (param.Item1.Files.Any(x => x.Info.FullName == newFilePath))
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
                        param.Item2.SetInfo(newFilePath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't change {param.Item2.Info.Name} to {newFileName}. Make sure the file is not opened by any process", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override bool Refresh()
        {
            bool canRefresh = base.Refresh();
            ShouldUpdate = canRefresh ? IsUpdateAvailable() : false;
            return canRefresh;
        }

        protected override bool CanCloseFileEditor(bool result, Sound fileBeforeEdit, Sound fileAfterEdit)
        {
            if (result)
            {
                //if (soundEvent.Files.Count(x => x.Name == fileAfterEdit.Name) > 1)
                {
                    Log.Warning("The sound already exists. Duplicates are not allowed", true);
                    return false;
                }
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

        protected override void OnFileEdited(bool result, Sound file)
        {
            if (result)
            {
                // TODO: Save changes
                //ChangeSoundPath(new Tuple<SoundEvent, Sound>(soundEvent, file));
                ForceJsonUpdate(); // temporary solution
            }
            else
            {
                // TODO: Undo commands
                base.OnFileEdited(result, file);
            }
            file.IsDirty = false;
        }

        protected override ObservableCollection<SoundEvent> FindFolders(string path, bool createRootIfEmpty = false)
        {
            try
            {
                string soundsFolder = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                ObservableCollection<SoundEvent> rootCollection = null;
                if (!File.Exists(path))
                {
                    File.AppendAllText(path, "{}");
                    return createRootIfEmpty ? CreateEmptyFoldersRoot(soundsFolder) : null;
                }
                Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

                rootCollection = JsonConvert.DeserializeObject<ObservableCollection<SoundEvent>>(File.ReadAllText(path), converter);

                if (createRootIfEmpty && (rootCollection == null || rootCollection.Count <= 0))
                {
                    rootCollection = CreateEmptyFoldersRoot(soundsFolder);
                }
                else if (rootCollection != null)
                {
                    foreach (SoundEvent item in rootCollection)
                    {
                        SetDelegates(item);
                    }
                }
                return rootCollection;
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
                return null;
            }
        }

        protected void SetDelegates(SoundEvent soundEvent)
        {
            try
            {
                soundEvent.CollectionChanged += (sender, args) => {
                    ShouldUpdate = CanRefresh() ? IsUpdateAvailable() : false;
                };
                soundEvent.OnFileAdded += AddSoundToJson;
                soundEvent.OnFileRemoved += RemoveSoundFromJson;
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
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
                    foreach (Sound sound in soundEvent.Files)
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
                string json = JsonConvert.SerializeObject(Folders, Preferences.JsonFormatting, converter);
                File.WriteAllText(FoldersRootPath, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected bool HasSoundWritten(string sound) => File.ReadLines(FoldersRootPath).Any(x => x.Contains($"\"{sound}\""));

        protected bool IsUpdateAvailable()
        {
            try
            {
                Converter.SoundCollectionConverter converter = new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                string newJson = JsonConvert.SerializeObject(Folders, Preferences.JsonFormatting);
                string savedJson = File.ReadAllText(FoldersRootPath);
                if (newJson == savedJson)
                {
                    newJson = JsonConvert.SerializeObject(Folders, Preferences.JsonFormatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
                }
                return newJson == savedJson;
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage);
                return false;
            }
        }
    }
}
