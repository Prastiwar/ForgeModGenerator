using ForgeModGenerator.Miscellaneous;
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

        public SoundJsonUpdater JsonUpdater { get; protected set; }

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(JsonUpdater.ForceJsonUpdate));

        protected override void AddNewFolder()
        {
            try
            {
                string soundsFolder = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                OpenFolderDialog.SelectedPath = soundsFolder;
                DialogResult dialogResult = OpenFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string newfolderPath = OpenFolderDialog.SelectedPath;
                    if (!IOExtensions.IsSubPathOf(newfolderPath, soundsFolder))
                    {
                        Log.Warning($"You can choose only folder from sounds folder ({soundsFolder})", true);
                        return;
                    }
                    SoundEvent newFolder = SoundEvent.CreateEmpty(newfolderPath);
                    SubscribeFolderEvents(newFolder);
                    AddNewFileToFolder(newFolder);
                    Folders.Add(newFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected void ChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            try
            {
                string modname = Mod.GetModnameFromPath(param.Item2.Info.FullName);
                string modid = Mod.GetModidFromPath(param.Item2.Name);
                string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
                string oldFilePath = param.Item2.Info.FullName.Replace("\\", "/");
                string extension = Path.GetExtension(oldFilePath);
                string newFileName = param.Item2.Name.Remove(0, param.Item2.Name.IndexOf(":") + 1); // remove modid
                string newFilePathToValidate = null;
                string newFilePath = null;
                try
                {
                    newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newFileName)}{extension}";
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
                        param.Item2.Name = Sound.FormatSoundPath(Mod.GetModidFromPath(param.Item2.Name), newFilePath);
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
            JsonUpdater = new SoundJsonUpdater(Folders, FoldersRootPath, Preferences.JsonFormatting, new Converter.SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
            ShouldUpdate = canRefresh ? JsonUpdater.IsUpdateAvailable() : false;
            return canRefresh;
        }

        protected override bool CanCloseFileEditor(bool result, FileEditedEventArgs args)
        {
            if (result)
            {
                if (args.Folder.Files.Count(x => x.Name == args.FileAfterEdit.Name) > 1)
                {
                    Log.Warning("The sound already exists. Duplicates are not allowed", true);
                    return false;
                }
            }
            else
            {
                if (args.FileAfterEdit.IsDirty)
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

        protected override void OnFileEdited(bool result, FileEditedEventArgs args)
        {
            if (result)
            {
                // TODO: Save changes
                if (args.FileBeforeEdit.Name != args.FileAfterEdit.Name)
                {
                    ChangeSoundPath(new Tuple<SoundEvent, Sound>(args.Folder, args.FileAfterEdit));
                    args.FileAfterEdit.RefreshName();
                }
                JsonUpdater.ForceJsonUpdate(); // temporary solution
            }
            else
            {
                // TODO: Undo commands
                base.OnFileEdited(result, args);
            }
            args.FileAfterEdit.IsDirty = false;
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
                        SubscribeFolderEvents(item);
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

        protected override void SubscribeFolderEvents(SoundEvent soundEvent)
        {
            try
            {
                soundEvent.CollectionChanged += (sender, args) => {
                    ShouldUpdate = CanRefresh() ? JsonUpdater.IsUpdateAvailable() : false;
                };
                soundEvent.PropertyChanged += (sender, args) => {
                    JsonUpdater.ForceJsonUpdate();
                };
                soundEvent.OnFileAdded += (sound) => { JsonUpdater.AddToJson(soundEvent, sound); };
                soundEvent.OnFileRemoved += (sound) => { JsonUpdater.RemoveFromJson(soundEvent, sound); };
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }
    }
}
