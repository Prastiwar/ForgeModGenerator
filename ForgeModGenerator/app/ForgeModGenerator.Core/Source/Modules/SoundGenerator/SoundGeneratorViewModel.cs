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
    public class SoundGeneratorViewModel : FoldersViewModelBase<SoundEvent, Sound>
    {
        // TODO: Refactor dependency mess
        public SoundGeneratorViewModel(ISessionContextService sessionContext,
                                        IDialogService dialogService,
                                        IFoldersExplorerFactory<SoundEvent, Sound> factory,
                                        IPreferenceService preferenceService,
                                        ISoundJsonUpdaterFactory jsonUpdaterFactory,
                                        IEditorFormFactory<Sound> editorFormFactory,
                                        IUniqueValidator<SoundEvent> soundEventValidator,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext, dialogService, factory)
        {
            Preferences = preferenceService.GetOrCreate<SoundsGeneratorPreferences>();

            PreferenceService = preferenceService;
            JsonUpdaterFactory = jsonUpdaterFactory;
            SoundEventValidator = soundEventValidator;
            CodeGenerationService = codeGenerationService;

            Explorer.AllowedFileExtensions.Add(".ogg");
            Explorer.OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;

            FileEditor = editorFormFactory.Create();
            FileEditor.ItemEdited += OnSoundEdited;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        protected IPreferenceService PreferenceService { get; }
        protected ISoundJsonUpdaterFactory JsonUpdaterFactory { get; }
        protected ICodeGenerationService CodeGenerationService { get; }
        protected IUniqueValidator<SoundEvent> SoundEventValidator { get; }

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = PreferenceService.GetOrCreate<SoundsGeneratorPreferences>();

                if (Explorer.Folders != null)
                {
                    Explorer.Folders.FilesChanged -= SubscribeFolderEvents;
                    Explorer.Folders.Clear();
                }

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
                Explorer.Folders.AddRange(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersJsonFilePath, true));
                Explorer.FileSynchronizer.RootPath = FoldersRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                RaisePropertyChanged(nameof(HasEmptyFolders));
                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<SoundEvent>(Explorer.Folders.Files, FileChange.Add));
                Explorer.Folders.FilesChanged += SubscribeFolderEvents;
                Explorer.Folders.FilesChanged += OnFoldersCollectionChanged;

                JsonUpdater = JsonUpdaterFactory.Create(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                JsonUpdater.SetTarget(Explorer.Folders.Files);
                JsonUpdater.Path = FoldersJsonFilePath;
                JsonUpdater.PrettyPrint = Preferences.SoundJsonPrettyPrint;

                CheckJsonFileMismatch();
                CheckForUpdate();
                return true;
            }
            return false;
        }

        protected override void ForceJsonFileUpdate()
        {
            base.ForceJsonFileUpdate();
            Mod mod = SessionContext.SelectedMod;
            CodeGenerationService.RegenerateInitScript(SourceCodeLocator.SoundEvents(mod.ModInfo.Name, mod.Organization).ClassName, mod, Explorer.Folders.Files);
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

        private string OnSoundEventValidate(SoundEvent sender, string propertyName) => SoundEventValidator.Validate(sender, Explorer.Folders.Files, propertyName).Error;

        private void OnSoundPropertyChanged(Sound file, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Sound.Name))
            {
                ForceJsonFileUpdate();
            }
        }

        private void OnSoundEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateResult validateResult = SoundEventValidator.Validate((SoundEvent)sender);
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
