using FluentValidation;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        private SoundEventValidator soundEventValidator;
        public SoundEventValidator SoundEventValidator => soundEventValidator ?? (soundEventValidator = new SoundEventValidator(Folders.Files));

        protected SoundCollectionConverter GetActualConverter() => new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = SessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
                ((SoundEventsSynchronizer)FileSynchronizer).SetModInfo(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                soundEventValidator = null; // will lazy load itself when needed
                if (Folders != null)
                {
                    Folders.OnFilesChanged -= SubscribeFolderEvents;
                    Folders.Clear();
                }
                Folders = new ObservableFolder<SoundEvent>(FoldersRootPath, await FileSynchronizer.FindFoldersAsync(FoldersJsonFilePath, true));
                SubscribeFolderEvents(Folders, new FileChangedEventArgs<SoundEvent>(Folders.Files, FileChange.Add));
                Folders.OnFilesChanged += SubscribeFolderEvents;
                Folders.OnFilesChanged += OnFoldersCollectionChanged;
                FileSynchronizer.Folders = Folders;
                JsonUpdater = new SoundJsonUpdater(Folders.Files, FoldersJsonFilePath, Preferences.JsonFormatting, GetActualConverter());
                CheckJsonFileMismatch();
                CheckForUpdate();
                return true;
            }
            return false;
        }

        protected override void ForceJsonFileUpdate()
        {
            base.ForceJsonFileUpdate();
            ;// TODO: GetCurrentSoundCodeGenerator().RegenerateScript();
        }

        protected void OnSoundEdited(object sender, SoundFileEditor.FileEditedEventArgs args)
        {
            if (args.Result)
            {
                ForceJsonFileUpdate();
            }
            else
            {
                args.ActualFile.CopyValues(args.CachedFile);
            }
            args.ActualFile.IsDirty = false;
        }

        protected void SubscribeFolderEvents(object sender, FileChangedEventArgs<SoundEvent> e)
        {
            if (e.Change == FileChange.Add)
            {
                foreach (SoundEvent soundEvent in e.Files)
                {
                    soundEvent.CollectionChanged += SoundEventSoundsChanged;
                    soundEvent.OnValidate += OnSoundEventValidate;
                    soundEvent.PropertyChanged += OnSoundEventPropertyChanged;
                    soundEvent.OnFilePropertyChanged += OnSoundPropertyChanged;
                }
            }
            else if (e.Change == FileChange.Remove)
            {
                foreach (SoundEvent soundEvent in e.Files)
                {
                    soundEvent.CollectionChanged -= SoundEventSoundsChanged;
                    soundEvent.OnValidate -= OnSoundEventValidate;
                    soundEvent.PropertyChanged -= OnSoundEventPropertyChanged;
                    soundEvent.OnFilePropertyChanged -= OnSoundPropertyChanged;
                }
            }
        }

        private void OnFoldersCollectionChanged(object sender, FileChangedEventArgs<SoundEvent> e)
        {
            ForceJsonFileUpdate();
            CheckForUpdate();
        }

        private string OnSoundEventValidate(SoundEvent sender, string propertyName) => new SoundEventValidator(Folders.Files).Validate(sender, propertyName).ToString();

        private void OnSoundPropertyChanged(Sound file, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Sound.Name))
            {
                ForceJsonFileUpdate();
            }
        }

        private void OnSoundEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FluentValidation.Results.ValidationResult validateResult = SoundEventValidator.Validate((SoundEvent)sender);
            if (validateResult.IsValid)
            {
                ForceJsonFileUpdate();
            }
        }

        private void SoundEventSoundsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ForceJsonFileUpdate();
            CheckForUpdate();
        }
    }
}
