using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FoldersJsonViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(GeneratorContext<Sound> context,
                                       IDialogService dialogService,
                                       IFoldersExplorerFactory<SoundEvent, Sound> factory,
                                       IPreferenceService preferenceService,
                                       ISoundJsonUpdaterFactory jsonUpdaterFactory,
                                       IUniqueValidator<SoundEvent> soundEventValidator,
                                       SoundEventChooseCollection soundEventChooseCollection)
            : base(context, dialogService, factory)
        {
            Preferences = preferenceService.GetOrCreate<SoundsGeneratorPreferences>();
            PreferenceService = preferenceService;
            JsonUpdaterFactory = jsonUpdaterFactory;
            SoundEventValidator = soundEventValidator;
            SoundEventChooseCollection = soundEventChooseCollection;

            Explorer.AllowFileExtensions(".ogg");
            Explorer.OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
        }

        public override string DirectoryRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        protected IPreferenceService PreferenceService { get; }
        protected ISoundJsonUpdaterFactory JsonUpdaterFactory { get; }
        protected SoundEventChooseCollection SoundEventChooseCollection { get; }
        protected IUniqueValidator<SoundEvent> SoundEventValidator { get; }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                Preferences = PreferenceService.GetOrCreate<SoundsGeneratorPreferences>();

                Explorer.Folders.FilesChanged -= SubscribeFolderEvents;
                Explorer.Folders.FilesChanged -= OnFoldersCollectionChanged;
                Explorer.Folders.Clear();

                if (Explorer.FileSynchronizer.Finder is SoundEventsFinder finder)
                {
                    finder.Initialize(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                }
                else
                {
                    throw new System.InvalidOperationException($"{Explorer.FileSynchronizer.Finder} must be {typeof(SoundEventsFinder)}");
                }
                if (Explorer.FileSynchronizer.Finder.Factory is ISoundEventsFactory factory)
                {
                    factory.SoundEventsRepository = Explorer.Folders;
                }
                else
                {
                    throw new System.InvalidOperationException($"{Explorer.FileSynchronizer.Finder} must be {typeof(SoundEventsFinder)}");
                }
                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersJsonFilePath, true).ConfigureAwait(true)).ConfigureAwait(false);
                Explorer.FileSynchronizer.RootPath = DirectoryRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                RaisePropertyChanged(nameof(HasEmptyFolders));
                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<SoundEvent>(Explorer.Folders.Files, FileChange.Add));
                Explorer.Folders.FilesChanged += SubscribeFolderEvents;
                Explorer.Folders.FilesChanged += OnFoldersCollectionChanged;

                JsonUpdater = JsonUpdaterFactory.Create(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                JsonUpdater.SetTarget(Explorer.Folders.Files);
                JsonUpdater.Path = FoldersJsonFilePath;
                JsonUpdater.PrettyPrint = Preferences.SoundJsonPrettyPrint;

                IsLoading = false;
                CheckJsonFileMismatch();
                CheckForUpdate();
                return true;
            }
            return false;
        }

        protected override void ForceJsonFileUpdate()
        {
            base.ForceJsonFileUpdate();
            McMod mcMod = SessionContext.SelectedMod;
            Context.CodeGenerationService.RegenerateInitScript(SourceCodeLocator.SoundEvents(mcMod.ModInfo.Name, mcMod.Organization).ClassName, mcMod, Explorer.Folders.Files);
        }
        protected override void OnItemEdited(object sender, ItemEditedEventArgs<Sound> args)
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
                foreach (Sound file in e.Files)
                {
                    SoundEventChooseCollection.RemoveGetter(GetGetterName(file.Name));
                }
            }
        }

        protected string GetGetterName(string modelName) =>
            SourceCodeLocator.SoundEvents(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).ClassName + "." + modelName;


        private void OnFoldersCollectionChanged(object sender, FileChangedEventArgs<SoundEvent> e)
        {
            ForceJsonFileUpdate();
            CheckForUpdate();
            RaisePropertyChanged(nameof(HasEmptyFolders));
        }

        private string OnSoundEventValidate(object sender, string propertyName) => SoundEventValidator.Validate((SoundEvent)sender, Explorer.Folders.Files, propertyName).Error;

        private void OnSoundPropertyChanged(Sound file, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Sound.Name))
            {
                ForceJsonFileUpdate();
            }
        }

        private void OnSoundEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateResult validateResult = SoundEventValidator.Validate((SoundEvent)sender, Explorer.Folders.Files);
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
