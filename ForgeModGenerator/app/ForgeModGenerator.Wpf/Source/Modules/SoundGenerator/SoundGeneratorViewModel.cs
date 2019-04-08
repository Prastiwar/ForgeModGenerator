using FluentValidation;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ForgeModGenerator.SoundGenerator.ViewModels
{
    /// <summary> SoundGenerator Business ViewModel </summary>
    public class SoundGeneratorViewModel : FoldersViewModelBase<SoundEvent, Sound>
    {
        public SoundGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService, IFoldersExplorerFactory<SoundEvent, Sound> factory) : base(sessionContext, dialogService)
        {
            explorer = factory.Create();
            explorer.AllowedFileExtensions.Add(".ogg");
            explorer.OpenFileDialog.Filter = "Sound file (*.ogg) | *.ogg";
            explorer.OpenFileDialog.Multiselect = true;
            explorer.OpenFileDialog.CheckFileExists = true;
            explorer.OpenFileDialog.ValidateNames = true;
            explorer.OpenFolderDialog.ShowNewFolderButton = true;
            FileEditor = new EditorForm<Sound>(Cache.Default, DialogService, new SoundEditForm());
            FileEditor.ItemEdited += OnSoundEdited;
        }

        private readonly IFoldersExplorer<SoundEvent, Sound> explorer;
        public override IFoldersExplorer<SoundEvent, Sound> Explorer => explorer;

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        private SoundEventValidator soundEventValidator;
        public SoundEventValidator SoundEventValidator => soundEventValidator ?? (soundEventValidator = new SoundEventValidator(explorer.Folders.Files));

        protected SoundCollectionConverter GetActualConverter() => new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = SessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
                soundEventValidator = null; // will lazy load itself when needed
                if (explorer.Folders != null)
                {
                    explorer.Folders.FilesChanged -= SubscribeFolderEvents;
                    explorer.Folders.Clear();
                }
                ((SoundEventsFinder)explorer.FileSynchronizer.Finder).Initialize(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                ((SoundEventsFactory)explorer.FileSynchronizer.Finder.Factory).SoundEvents = explorer.Folders;
                explorer.Folders.AddRange(await explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersJsonFilePath, true));
                explorer.FileSynchronizer.RootPath = FoldersRootPath;
                explorer.FileSynchronizer.SynchronizingObject = SyncInvokeObject.Default;
                explorer.FileSynchronizer.SetEnableSynchronization(true);
                RaisePropertyChanged(nameof(HasEmptyFolders));
                SubscribeFolderEvents(explorer.Folders, new FileChangedEventArgs<SoundEvent>(explorer.Folders.Files, FileChange.Add));
                explorer.Folders.FilesChanged += SubscribeFolderEvents;
                explorer.Folders.FilesChanged += OnFoldersCollectionChanged;
                JsonUpdater = new SoundJsonUpdater(explorer.Folders.Files, FoldersJsonFilePath, Preferences.JsonFormatting, GetActualConverter());
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

        private string OnSoundEventValidate(SoundEvent sender, string propertyName) => new SoundEventValidator(explorer.Folders.Files).Validate(sender, propertyName).ToString();

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
