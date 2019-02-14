using ForgeModGenerator.Converters;
using ForgeModGenerator.ModGenerator.Models;
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
            AllowedFileExtensions = new string[] { ".ogg" };
            FileEditForm = new SoundEditForm();
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
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(FindAndAddNewFiles));

        protected override void AddNewFolder()
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
                if (newFolder.Count > 0)
                {
                    Folders.Add(newFolder);
                }
            }
        }

        protected void ChangeSoundPath(Tuple<SoundEvent, Sound> param)
        {
            System.Windows.Controls.ValidationResult validation = param.Item2.IsValid(param.Item1.Files);
            if (!validation.IsValid)
            {
                return;
            }
            string soundsFolderPath = ModPaths.SoundsFolder(Mod.GetModnameFromPath(param.Item2.Info.FullName), Mod.GetModidFromPath(param.Item2.Name));
            string oldFilePath = param.Item2.Info.FullName.Replace("\\", "/");
            string relativePath = Sound.GetRelativePathFromSoundPath(param.Item2.Name);
            string newFileName = relativePath.EndsWith("/") ? relativePath.Substring(0, relativePath.Length - 1) : relativePath;
            string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newFileName)}{Path.GetExtension(oldFilePath)}";
            string newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");

            if (oldFilePath != newFilePath)
            {
                if (param.Item1.Files.Any(x => x.Info.FullName == newFilePath))
                {
                    Log.Warning($"{param.Item1.EventName} already has this sound", true);
                    return;
                }
                if (RenameFile(param.Item2, newFilePath))
                {
                    string formattedSound = Sound.FormatSoundPath(Mod.GetModidFromPath(param.Item2.Name), newFileName);
                    param.Item2.Name = formattedSound;
                }
            }
        }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override bool Refresh()
        {
            bool canRefresh = base.Refresh();
            if (canRefresh)
            {
                JsonUpdater = new SoundJsonUpdater(Folders, FoldersRootPath, Preferences.JsonFormatting, new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
                CheckForUpdate();
            }
            return canRefresh;
        }

        protected bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                return EnumerateFilteredFiles(soundsFolderPath, System.IO.SearchOption.AllDirectories).All(filePath => ReferenceCounter.IsReferenced(filePath));
            }
            return true;
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !ReferenceCounter.IsReferenced(filePath)))
            {
                SoundEvent newFolder = new SoundEvent(SessionContext.SelectedMod.ModInfo.Modid, filePath);
                string cachedName = newFolder.EventName;
                int i = 1;
                while (ReferenceCounter.GetReferenceCount(newFolder.EventName) > 1)
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
                System.Windows.Controls.ValidationResult validation = args.ActualFile.IsValid(args.Folder.Files);
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
                    args.ActualFile.FormatName();
                }
                ForceUpdate();
            }
            else
            {
                base.OnFileEdited(result, args);
            }
            args.ActualFile.IsDirty = false;
        }

        protected void CheckForUpdate() => ShouldUpdate = !IsJsonUpdated();

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
                    JsonUpdater = new SoundJsonUpdater(folders, FoldersRootPath, Preferences.JsonFormatting, new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
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

        protected void ForceUpdate() => JsonUpdater.ForceJsonUpdate();// GetCurrentSoundCodeGenerator().RegenerateScript();
    }
}
