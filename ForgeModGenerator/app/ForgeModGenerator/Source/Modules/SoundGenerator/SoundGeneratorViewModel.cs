using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.Utility;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FolderListViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService) : base(sessionContext, dialogService)
        {
            if (IsInDesignMode)
            {
                return;
            }
            OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ogg" };
            FileEditForm = new SoundEditForm();
            FileEditor = new SoundFileEditor(dialogService, FileEditForm);
            FileEditor.OnFileEdited += OnSoundEdited;
            FileSynchronizer = SessionContext.IsModSelected
                             ? new SoundEventsSynchronizer(Folders, SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid, FoldersRootPath, AllowedFileExtensionsPatterns)
                             : new SoundEventsSynchronizer(Folders, "", "", FoldersRootPath, AllowedFileExtensionsPatterns);
            FileSynchronizer.FolderInstantiated += SubscribeFolderEvents;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersSerializeFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        protected SoundsGeneratorPreferences Preferences { get; set; }

        private SoundEventValidator soundEventValidator;
        public SoundEventValidator SoundEventValidator => soundEventValidator ?? (soundEventValidator = new SoundEventValidator(Folders));

        private SoundJsonUpdater jsonUpdater;
        protected SoundJsonUpdater JsonUpdater => jsonUpdater ?? (jsonUpdater = new SoundJsonUpdater(Folders, FoldersSerializeFilePath, Preferences.JsonFormatting, GetActualConverter()));

        private ICommand updateSoundsJson;
        public ICommand UpdateSoundsJson => updateSoundsJson ?? (updateSoundsJson = new RelayCommand(FindAndAddNewFiles));

        protected SoundCollectionConverter GetActualConverter() => new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

        protected bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                return FileSynchronizer.EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in FileSynchronizer.EnumerateFilteredFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath)))
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                if (!FileSynchronizer.TryGetFolder(dirPath, out SoundEvent folder))
                {
                    folder = new SoundEvent(dirPath) {
                        EventName = IOHelper.GetUniqueName(SoundEvent.FormatDottedSoundNameFromFullPath(filePath), (name) => Folders.All(inFolder => inFolder.EventName != name))
                    };
                    SubscribeFolderEvents(this, folder);
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

        protected override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = SessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
                (FileSynchronizer as SoundEventsSynchronizer).SetModInfo(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                jsonUpdater = null; // will lazy load itself when needed
                soundEventValidator = null; // will lazy load itself when needed
                Folders = await FileSynchronizer.FindFolders(FoldersSerializeFilePath, true);
                CheckSerializationFileMismatch(FoldersSerializeFilePath);
                CheckForUpdate();
                return true;
            }
            return false;
        }

        /// <summary> Deserialized folders from "filePath" and checks if any file doesn't exists, if so, prompt if should fix this </summary>
        protected async void CheckSerializationFileMismatch(string filePath)
        {
            ObservableCollection<SoundEvent> deserializedFolders = await FileSynchronizer.FindFoldersFromFile(filePath, false);
            bool hasNotExistingFile = deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName)));
            if (hasNotExistingFile)
            {
                string questionMessage = "sounds.json file has occurencies that doesn't exists in sounds folder. Do you want to fix it and overwrite sounds.json?";
                bool shouldFix = await DialogService.ShowMessage(questionMessage, "Conflict found", "Yes", "No", null);
                if (shouldFix)
                {
                    ForceUpdate();
                }
            }
        }

        protected void OnSoundEdited(object sender, SoundFileEditor.FileEditedEventArgs args)
        {
            if (args.Result)
            {
                ForceUpdate();
            }
            else
            {
                args.ActualFile.CopyValues(args.CachedFile);
            }
            args.ActualFile.IsDirty = false;
        }

        protected void SubscribeFolderEvents(object sender, SoundEvent soundEvent)
        {
            soundEvent.CollectionChanged += (s, args) => {
                ForceUpdate();
                CheckForUpdate();
            };
            soundEvent.OnValidate += (s, propertyName) => {
                return new SoundEventValidator(Folders).Validate(s, propertyName).ToString();
            };
            soundEvent.PropertyChanged += (s, args) => {
                FluentValidation.Results.ValidationResult validateResult = SoundEventValidator.Validate((SoundEvent)s);
                if (validateResult.IsValid)
                {
                    ForceUpdate();
                }
            };
            soundEvent.OnFilePropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Sound.Name))
                {
                    ForceUpdate();
                }
            };
        }
    }
}
