using FluentValidation;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.ViewModels;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FoldersViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService, IFoldersExplorerFactory<SoundEvent, Sound> factory, IPreferenceService preferenceService) : base(sessionContext, dialogService, factory)
        {
            PreferenceService = preferenceService;
            Preferences = preferenceService.GetOrCreate<SoundsGeneratorPreferences>();
            Explorer.AllowedFileExtensions.Add(".ogg");
            Explorer.OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            FileEditor = new EditorForm<Sound>(Cache.Default, DialogService, new SoundEditForm());
            FileEditor.ItemEdited += OnSoundEdited;
        }

        protected IPreferenceService PreferenceService { get; }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        private SoundEventValidator soundEventValidator;
        public SoundEventValidator SoundEventValidator => soundEventValidator ?? (soundEventValidator = new SoundEventValidator(Explorer.Folders.Files));

        protected SoundCollectionConverter GetActualConverter() => new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = PreferenceService.GetOrCreate<SoundsGeneratorPreferences>();
                soundEventValidator = null; // will lazy load itself when needed
                if (Explorer.Folders != null)
                {
                    Explorer.Folders.FilesChanged -= SubscribeFolderEvents;
                    Explorer.Folders.Clear();
                }
                ((SoundEventsFinder)Explorer.FileSynchronizer.Finder).Initialize(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                ((SoundEventsFactory)Explorer.FileSynchronizer.Finder.Factory).SoundEvents = Explorer.Folders;
                Explorer.Folders.AddRange(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersJsonFilePath, true));
                Explorer.FileSynchronizer.RootPath = FoldersRootPath;
                Explorer.FileSynchronizer.SynchronizingObject = SyncInvokeObject.Default;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                RaisePropertyChanged(nameof(HasEmptyFolders));
                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<SoundEvent>(Explorer.Folders.Files, FileChange.Add));
                Explorer.Folders.FilesChanged += SubscribeFolderEvents;
                Explorer.Folders.FilesChanged += OnFoldersCollectionChanged;
                JsonUpdater = new SoundJsonUpdater(Explorer.Folders.Files, FoldersJsonFilePath, Preferences.SoundJsonPrettyPrint ? Formatting.Indented : Formatting.None, GetActualConverter());
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

        protected void OnSoundEdited(object sender, ItemEditedEventArgs<Sound> args)
        {
            if (args.Result)
            {
                ForceJsonFileUpdate();
            }
            else
            {
                args.ActualItem.CopyValues(args.CachedItem);
            }
            args.ActualItem.IsDirty = false;
        }

        protected void SubscribeFolderEvents(object sender, FileChangedEventArgs<SoundEvent> e)
        {
            if (e.Change == FileChange.Add)
            {
                foreach (SoundEvent soundEvent in e.Files)
                {
                    soundEvent.CollectionChanged += SoundEventSoundsChanged;
                    soundEvent.ValidateProperty += OnSoundEventValidate;
                    soundEvent.PropertyChanged += OnSoundEventPropertyChanged;
                    soundEvent.FilePropertyChanged += OnSoundPropertyChanged;
                    soundEvent.FilesChanged += OnSoundEventFileChanged;
                }
            }
            else if (e.Change == FileChange.Remove)
            {
                foreach (SoundEvent soundEvent in e.Files)
                {
                    soundEvent.CollectionChanged -= SoundEventSoundsChanged;
                    soundEvent.ValidateProperty -= OnSoundEventValidate;
                    soundEvent.PropertyChanged -= OnSoundEventPropertyChanged;
                    soundEvent.FilePropertyChanged -= OnSoundPropertyChanged;
                    soundEvent.FilesChanged -= OnSoundEventFileChanged;
                }
            }
        }

        private void OnSoundEventFileChanged(object sender, FileChangedEventArgs<Sound> e)
        {
            if (e.Change == FileChange.Remove)
            {
                if (((SoundEvent)sender).Count == 0)
                {
                    RaisePropertyChanged(nameof(HasEmptyFolders));
                }
            }
        }

        private void OnFoldersCollectionChanged(object sender, FileChangedEventArgs<SoundEvent> e)
        {
            ForceJsonFileUpdate();
            CheckForUpdate();
            RaisePropertyChanged(nameof(HasEmptyFolders));
        }

        private string OnSoundEventValidate(SoundEvent sender, string propertyName) => new SoundEventValidator(Explorer.Folders.Files).Validate(sender, propertyName).ToString();

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
