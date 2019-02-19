﻿using FluentValidation;
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
        public SoundGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService) : base(sessionContext, dialogService)
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

        private bool shouldUpdate;
        public bool ShouldUpdate {
            get => shouldUpdate;
            set => Set(ref shouldUpdate, value);
        }

        protected SoundsGeneratorPreferences Preferences { get; }

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
                return EnumerateAllowedFiles(soundsFolderPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }

        protected void FindAndAddNewFiles()
        {
            string soundsFolderPath = ModPaths.SoundsFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid);
            foreach (string filePath in EnumerateAllowedFiles(soundsFolderPath, SearchOption.AllDirectories).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath)))
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                if (!TryGetFolder(dirPath, out SoundEvent folder))
                {
                    folder = new SoundEvent(dirPath) {
                        EventName = IOHelper.GetUniqueName(SoundEvent.FormatDottedSoundNameFromFullPath(filePath), (name) => Folders.All(inFolder => inFolder.EventName != name))
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
                jsonUpdater = null; // will lazy load itself when needed
                soundEventValidator = null; // will lazy load itself when needed
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
        protected async void CheckSerializationFileMismatch(string filePath)
        {
            ObservableCollection<SoundEvent> deserializedFolders = FindFoldersFromFile(filePath, false);
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
            soundEvent.OnValidate += (sender, propertyName) => {
                return new SoundEventValidator(Folders).Validate(sender, propertyName).ToString();
            };
            soundEvent.PropertyChanged += (sender, args) => {
                FluentValidation.Results.ValidationResult validateResult = SoundEventValidator.Validate((SoundEvent)sender);
                if (validateResult.IsValid)
                {
                    ForceUpdate();
                }
            };
        }
    }
}
