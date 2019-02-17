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
                return EnumerateAllowedFiles(soundsFolderPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in EnumerateAllowedFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath)))
            {
                string dirPath = IOExtensions.GetDirectoryPath(filePath);
                if (!TryGetFolder(dirPath, out SoundEvent folder))
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
                CheckSerializationFileMismatch(FoldersSerializeFilePath);
                CheckForUpdate();
                return true;
            }
            return false;
        }

        protected override void OnFileEditorOpening(object sender, FileEditorOpeningDialogEventArgs eventArgs) => (FileEditForm as SoundEditForm).AllSounds = eventArgs.Folder.Files;

        protected override void OnFileEdited(bool result, FileEditedEventArgs args)
        {
            if (result)
            {
                ForceUpdate();
            }
            else
            {
                base.OnFileEdited(result, args);
            }
            args.ActualFile.IsDirty = false;
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
            return hasNotExistingFile ? FilterToOnlyExistingFiles(deserializedFolders) : deserializedFolders;
        }

        /// <summary> Deserialized folders from "filePath" and checks if any file doesn't exists, if so, prompt if should fix this </summary>
        protected void CheckSerializationFileMismatch(string filePath)
        {
            ObservableCollection<SoundEvent> deserializedFolders = FindFoldersFromFile(filePath, false);
            bool hasNotExistingFile = deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName)));
            if (hasNotExistingFile)
            {
                DialogResult result = MessageBox.Show("sounds.json file has occurencies that doesn't exists in sounds folder. Do you want to fix it and overwrite sounds.json?", "Conflict found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ForceUpdate();
                }
            }
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
