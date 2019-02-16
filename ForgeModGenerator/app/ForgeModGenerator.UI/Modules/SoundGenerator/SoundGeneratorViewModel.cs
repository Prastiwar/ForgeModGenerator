using ForgeModGenerator.Converters;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utils;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            if (IsInDesignMode)
            {
                return;
            }
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ogg" };
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
            string relativePath = Sound.GetRelativePathFromSoundName(param.Item2.Name);
            string newRelativePath = relativePath.EndsWith("/") ? relativePath.Substring(0, relativePath.Length - 1) : relativePath;
            string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newRelativePath)}{Path.GetExtension(oldFilePath)}";
            string newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");

            if (oldFilePath != newFilePath)
            {
                FileInfo newFile = new FileInfo(newFilePath);
                newFile.Directory.Create();
                File.Move(param.Item2.Info.FileSystemInfo.FullName, newFile.FullName);
            }
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath)))
            {
                string dirPath = IOExtensions.GetDirectoryPath(filePath);
                SoundEvent folder = Folders.Find(x => x.Info.FullName == dirPath);
                if (folder == null)
                {
                    folder = new SoundEvent(dirPath) {
                        EventName = IOExtensions.GetUniqueName(SoundEvent.FormatDottedSoundNameFromFullPath(filePath), (name) => Folders.All(inFolder => inFolder.EventName != name))
                    };
                    SubscribeFolderEvents(folder);
                    Folders.Add(folder);
                }
                folder.Add(filePath);
            }
            ForceUpdate();
            CheckForUpdate();
        }

        protected void CheckForUpdate() => ShouldUpdate = !IsJsonUpdated();

        protected void ForceUpdate() => JsonUpdater?.ForceJsonUpdate();// GetCurrentSoundCodeGenerator().RegenerateScript();

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
            base.AddNewFolder();
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
