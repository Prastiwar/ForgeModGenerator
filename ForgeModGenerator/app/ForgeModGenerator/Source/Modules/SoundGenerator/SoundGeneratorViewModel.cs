using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Persistence;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.ViewModels;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
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
            FileSynchronizer.FolderInstantiated += SubscribeFolderEvents;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override string FoldersJsonFilePath => SessionContext.SelectedMod != null ? ModPaths.SoundsJson(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        protected SoundsGeneratorPreferences Preferences { get; set; }

        private SoundEventValidator soundEventValidator;
        public SoundEventValidator SoundEventValidator => soundEventValidator ?? (soundEventValidator = new SoundEventValidator(Folders));

        protected SoundCollectionConverter GetActualConverter() => new SoundCollectionConverter(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);

        protected override bool CanRefresh() => SessionContext.SelectedMod != null;

        protected override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                Preferences = SessionContext.GetOrCreatePreferences<SoundsGeneratorPreferences>();
                (FileSynchronizer as SoundEventsSynchronizer).SetModInfo(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
                soundEventValidator = null; // will lazy load itself when needed
                Folders = await FileSynchronizer.FindFolders(FoldersJsonFilePath, true);
                JsonUpdater = new SoundJsonUpdater(Folders, FoldersJsonFilePath, Preferences.JsonFormatting, GetActualConverter());
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

        protected void SubscribeFolderEvents(object sender, SoundEvent soundEvent)
        {
            soundEvent.CollectionChanged += (s, args) => {
                ForceJsonFileUpdate();
                CheckForUpdate();
            };
            soundEvent.OnValidate += (s, propertyName) => {
                return new SoundEventValidator(Folders).Validate(s, propertyName).ToString();
            };
            soundEvent.PropertyChanged += (s, args) => {
                FluentValidation.Results.ValidationResult validateResult = SoundEventValidator.Validate((SoundEvent)s);
                if (validateResult.IsValid)
                {
                    ForceJsonFileUpdate();
                }
            };
            soundEvent.OnFilePropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Sound.Name))
                {
                    ForceJsonFileUpdate();
                }
            };
        }
    }
}
