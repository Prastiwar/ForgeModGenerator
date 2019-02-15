using ForgeModGenerator.Converters;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utils;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FolderListViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext) : base(sessionContext)
        {
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedFileExtensions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ogg" };
            FileEditForm = new SoundEditForm();
            Preferences = sessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
            Refresh();
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersSerializeFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public SoundsGeneratorPreferences Preferences { get; }

        public SoundJsonUpdater JsonUpdater { get; protected set; }

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(FindAndAddNewFiles));

        protected bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                return EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }

        protected void ChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            ValidationResult validation = param.Item2.IsValid(param.Item1.Files);
            if (!validation.IsValid)
            {
                return;
            }
            string soundsFolderPath = param.Item2.GetSoundsFolder();
            string oldFilePath = param.Item2.Info.FullName;
            string relativePath = param.Item2.ShortPath;
            string newRelativePath = relativePath.EndsWith("/") ? relativePath.Substring(0, relativePath.Length - 1) : relativePath;
            string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newRelativePath)}{Path.GetExtension(oldFilePath)}";
            string newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");

            if (oldFilePath != newFilePath)
            {
                if (!param.Item2.Info.Rename(newFilePath))
                {
                    Log.Warning($"Couldn't rename {param.Item2.Info.Name} to {newFilePath}", true);
                }
            }
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath)))
            {
                SoundEvent newFolder = new SoundEvent(SessionContext.SelectedMod.ModInfo.Modid, filePath);
                string cachedName = newFolder.EventName;
                int i = 1;
                while (Folders.Any(folder => folder.EventName == newFolder.EventName))
                {
                    newFolder.EventName = $"{cachedName}({i})";
                    i++;
                }
                SubscribeFolderEvents(newFolder);
                Folders.Add(newFolder);
            }
            ForceUpdate();
            CheckForUpdate();
        }

        protected void CheckForUpdate() => ShouldUpdate = !IsJsonUpdated();

        protected void ForceUpdate() => JsonUpdater.ForceJsonUpdate();// GetCurrentSoundCodeGenerator().RegenerateScript();

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override bool Refresh()
        {
            if (CanRefresh())
            {
                Folders = FindFolders(FoldersSerializeFilePath, true);
                JsonUpdater = new SoundJsonUpdater(Folders, FoldersSerializeFilePath, Preferences.JsonFormatting, new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
                CheckForUpdate();
                return true;
            }
            return false;
        }

        protected override void OnFileEditorOpening(object sender, FileEditorOpeningDialogEventArgs eventArgs) => (FileEditForm as SoundEditForm).AllSounds = eventArgs.Folder.Files;

        protected override bool CanCloseFileEditor(bool result, FileEditedEventArgs args)
        {
            if (result)
            {
                if (!(FileEditForm as SoundEditForm).IsValid())
                {
                    Log.Warning($"Cannot save, your data is not valid", true);
                    return false;
                }
                ValidationResult validation = args.ActualFile.IsValid(args.Folder.Files);
                if (!validation.IsValid)
                {
                    Log.Warning($"Cannot save, {validation.ErrorContent}", true);
                    return false;
                }
            }
            else
            {
                if (args.ActualFile.IsDirty)
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
                if (args.CachedFile.Name != args.ActualFile.Name)
                {
                    ChangeSoundPath(new Tuple<SoundEvent, Sound>(args.Folder, args.ActualFile));
                }
                ForceUpdate();
            }
            else
            {
                base.OnFileEdited(result, args);
            }
            args.ActualFile.IsDirty = false;
        }

        protected override void AddNewFolder()
        {
            string soundsFolder = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            OpenFolderDialog.SelectedPath = soundsFolder;
            DialogResult dialogResult = OpenFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string newfolderPath = OpenFolderDialog.SelectedPath;
                // TODO: Allow adding folder from other path than soundsFolder (so add all sounds from there)
                if (!IOExtensions.IsSubPathOf(newfolderPath, soundsFolder))
                {
                    Log.Warning($"You can choose only folder from sounds folder ({soundsFolder})", true);
                    return;
                }
                SoundEvent newFolder = SoundEvent.CreateEmpty(newfolderPath);
                SubscribeFolderEvents(newFolder);
                AddNewFileToFolder(newFolder);
                if (newFolder.Count > 0)
                {
                    Folders.Add(newFolder);
                }
            }
        }

        protected override ObservableCollection<SoundEvent> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return new ObservableCollection<SoundEvent>();
            }
            ObservableCollection<SoundEvent> deserializedFolders = FindFoldersFromFile(path, false);
            bool hasNotExistingFile = deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName)));
            ObservableCollection<SoundEvent> folders = hasNotExistingFile ? FilterExistingFiles(deserializedFolders) : deserializedFolders;
            if (hasNotExistingFile)
            {
                DialogResult result = MessageBox.Show("sounds.json file has occurencies that doesn't exists in sounds folder. Do you want to fix and overwrite sounds.json?", "Conflict found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    JsonUpdater = new SoundJsonUpdater(folders, FoldersSerializeFilePath, Preferences.JsonFormatting, new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
                    ForceUpdate();
                }
            }
            return folders;
        }

        protected override ObservableCollection<SoundEvent> DeserializeFolders(string fileCotent)
        {
            SoundCollectionConverter converter = new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            return JsonConvert.DeserializeObject<ObservableCollection<SoundEvent>>(fileCotent, converter);
        }

        protected override void SubscribeFolderEvents(SoundEvent soundEvent)
        {
            soundEvent.CollectionChanged += (sender, args) => {
                ForceUpdate();
                CheckForUpdate();
            };
            soundEvent.PropertyChanged += (sender, args) => {
                ForceUpdate();
            };
        }
    }
}
