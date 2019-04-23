using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.ViewModels
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : BindableBase
    {
        public ModGeneratorViewModel(ISessionContextService sessionContext,
                                     IDialogService dialogService,
                                     IFileSystem fileSystem,
                                     IEditorFormFactory<McMod> editorFormFactory,
                                     IValidator<McMod> modValidator,
                                     ICodeGenerationService codeGenerationService,
                                     ISerializer<McMod> modSerializer,
                                     ISerializer<McModInfo> modInfoSerializer)
        {
            SessionContext = sessionContext;
            DialogService = dialogService;
            FileSystem = fileSystem;
            EditorFormFactory = editorFormFactory;
            ModValidator = modValidator;
            CodeGenerationService = codeGenerationService;
            ModSerializer = modSerializer;
            ModInfoSerializer = modInfoSerializer;
            ResetNewMod();
            EditorForm = editorFormFactory.Create();
            EditorForm.Validator = modValidator;
            EditorForm.ItemEdited += Editor_OnItemEdited;
        }

        private readonly string[] assetsFolerToGenerate = new string[] {
            "blockstates", "lang", "recipes", "sounds", "models/item", "textures/blocks", "textures/entity", "textures/items", "textures/models/armor"
        };

        protected IValidator<McMod> ModValidator { get; set; }
        protected IEditorForm<McMod> EditorForm { get; set; }
        protected IEditorFormFactory<McMod> EditorFormFactory { get; }
        protected ICodeGenerationService CodeGenerationService { get; }
        protected ISerializer<McMod> ModSerializer { get; }
        protected ISerializer<McModInfo> ModInfoSerializer { get; }

        public ISessionContextService SessionContext { get; }
        public IDialogService DialogService { get; }
        public IFileSystem FileSystem { get; }

        public IEnumerable<ModSide> Sides => Enum.GetValues(typeof(ModSide)).Cast<ModSide>();

        private McMod newMod;
        public McMod NewMod {
            get => newMod;
            set => SetProperty(ref newMod, value);
        }

        private McMod selectedEditMod;
        public McMod SelectedEditMod {
            get => selectedEditMod;
            set => SetProperty(ref selectedEditMod, value);
        }

        private ICommand createModCommand;
        public ICommand CreateModCommand => createModCommand ?? (createModCommand = new DelegateCommand(() => CreateNewMod(NewMod)));

        private ICommand removeModCommand;
        public ICommand RemoveModCommand => removeModCommand ?? (removeModCommand = new DelegateCommand<McMod>(RemoveMod));

        private ICommand editModCommand;
        public ICommand EditModCommand => editModCommand ?? (editModCommand = new DelegateCommand<McMod>(EditMod));

        private ICommand showModContainerCommand;
        public ICommand ShowModContainerCommand => showModContainerCommand ?? (showModContainerCommand = new DelegateCommand<McMod>(ShowContainer));

        private void ShowContainer(McMod mcMod) => Process.Start(ModPaths.ModRootFolder(mcMod.ModInfo.Name));

        private string OnModValidate(McMod sender, string propertyName) => ModValidator.Validate(sender, propertyName).Error;

        private async void RemoveMod(McMod mcMod)
        {
            bool shouldRemove = await DialogService.ShowMessage("Are you sure you want to delete this mod? Folder will be moved to trash bin", "Confirm deletion", "Yes", "No", null);
            if (shouldRemove)
            {
                if (SessionContext.Mods.Remove(mcMod))
                {
                    try
                    {
                        FileSystem.DeleteDirectory(ModPaths.ModRootFolder(mcMod.ModInfo.Name), true);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Couldn't remove mod. Make sure folder or its file is not used in any other process", true);
                        SessionContext.Mods.Add(mcMod); // remove failed, so rollback
                    }
                }
            }
        }

        private void ResetNewMod() => NewMod = new McMod(new McModInfo() {
            Name = "NewExampleMod",
            Modid = "newexamplemod",
            Credits = "For contributors of ForgeModGenerator",
            Description = "This is example mod",
            McVersion = "12.2",
            Version = "1.0",
            AuthorList = new ObservableCollection<string>() { "Me" },
            Dependencies = new ObservableCollection<string>(),
            Screenshots = new ObservableCollection<string>()
        }, "newexampleorg", SessionContext.ForgeVersions[0]);

        private void EditMod(McMod mcMod)
        {
            SelectedEditMod = mcMod;
            mcMod.ValidateProperty += OnModValidate;
            EditorForm.OpenItemEditor(mcMod);
        }

        private void CreateNewMod(McMod mcMod)
        {
            IEditorForm<McMod> tempEditor = EditorFormFactory.Create();
            tempEditor.Validator = ModValidator;
            tempEditor.ItemEdited += (sender, e) => {
                if (e.Result)
                {
                    GenerateMod(e.ActualItem);
                }
                mcMod.ValidateProperty -= OnModValidate;
            };
            mcMod.ValidateProperty += OnModValidate;
            tempEditor.OpenItemEditor(mcMod);
        }

        private void Editor_OnItemEdited(object sender, ItemEditedEventArgs<McMod> e)
        {
            if (e.Result)
            {
                if (e.ActualItem.Validate().IsValid)
                {
                    SaveModChanges(e.ActualItem);
                }
            }
            else
            {
                e.ActualItem.CopyValues(e.CachedItem);
            }
            e.ActualItem.ValidateProperty -= OnModValidate;
        }

        private void GenerateMod(McMod mcMod)
        {
            ValidateResult validation = ModValidator.Validate(mcMod);
            if (!validation.IsValid)
            {
                Log.Warning($"Selected mod is not valid. Reason: {validation}", true);
                return;
            }
            string newModPath = ModPaths.ModRootFolder(mcMod.ModInfo.Name);
            if (Directory.Exists(newModPath))
            {
                Log.Warning($"{newModPath} already exists", true);
                return;
            }
            mcMod.ForgeVersion.UnZip(newModPath);
            RemoveDumpExample(mcMod);

            string assetsPath = ModPaths.AssetsFolder(mcMod.ModInfo.Name, mcMod.ModInfo.Modid);
            IOHelper.GenerateFolders(assetsPath, assetsFolerToGenerate);
            CodeGenerationService.RegenerateSourceCode(mcMod);

            mcMod.WorkspaceSetup.Setup(mcMod);
            ModHelper.ExportMcInfo(ModInfoSerializer, mcMod.ModInfo);
            ModHelper.ExportMod(ModSerializer, mcMod);

            SessionContext.Mods.Add(mcMod);
            Log.Info($"{mcMod.ModInfo.Name} was created successfully", true);
        }

        private void RemoveDumpExample(McMod mcMod)
        {
            string javaSource = ModPaths.JavaSourceFolder(mcMod.ModInfo.Name);
            foreach (string organization in Directory.EnumerateDirectories(javaSource))
            {
                FileSystem.DeleteDirectory(organization, false);
            }
        }

        private void SaveModChanges(McMod mcMod)
        {
            ValidateResult validation = ModValidator.Validate(mcMod);
            if (!validation.IsValid)
            {
                Log.Warning($"Selected mod is not valid. Reason: {validation}", true);
                return;
            }
            McMod oldValues = ModHelper.ImportMod(ModSerializer, ModPaths.ModRootFolder(mcMod.CachedName));
            try
            {
                bool organizationChanged = mcMod.Organization != oldValues.Organization;
                bool nameChanged = mcMod.ModInfo.Name != oldValues.ModInfo.Name;
                bool modidChanged = mcMod.ModInfo.Modid != oldValues.ModInfo.Modid;

                if (organizationChanged)
                {
                    string oldOrganizationPath = ModPaths.OrganizationRootFolder(oldValues.ModInfo.Name, oldValues.Organization);
                    if (!FileSystem.RenameDirectory(oldOrganizationPath, mcMod.Organization))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldOrganizationPath), "Rename failed");
                        mcMod.Organization = oldValues.Organization;
                    }
                }
                if (modidChanged)
                {
                    string oldAssetPath = ModPaths.AssetsFolder(oldValues.ModInfo.Name, oldValues.ModInfo.Modid);
                    if (!FileSystem.RenameDirectory(oldAssetPath, mcMod.ModInfo.Modid))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldAssetPath), "Rename failed");
                        mcMod.ModInfo.Modid = oldValues.ModInfo.Modid;
                    }
                }
                if (nameChanged)
                {
                    bool canChangeName = true;
                    string oldSourceCodePath = ModPaths.SourceCodeRootFolder(oldValues.ModInfo.Name, mcMod.Organization);
                    if (!FileSystem.RenameDirectory(oldSourceCodePath, mcMod.ModInfo.Name.ToLower()))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldSourceCodePath), "Rename failed");
                        mcMod.Name = oldValues.Name;
                    }

                    string oldNamePath = ModPaths.ModRootFolder(oldValues.ModInfo.Name);
                    if (canChangeName && !FileSystem.RenameDirectory(oldNamePath, mcMod.ModInfo.Name))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldNamePath), "Rename failed");
                        string sourceCodeParentPath = new DirectoryInfo(oldSourceCodePath).Parent.FullName;
                        string newSourceCodePath = Path.Combine(sourceCodeParentPath, mcMod.ModInfo.Name.ToLower());
                        FileSystem.RenameDirectory(newSourceCodePath, oldValues.Name.ToLower());
                        mcMod.Name = oldValues.Name;
                    }
                }

                bool forgeVersionChanged = mcMod.ForgeVersion.Name != oldValues.ForgeVersion.Name;
                if (forgeVersionChanged)
                {
                    ChangeForgeVersion(mcMod);
                }

                bool workspaceChanged = mcMod.WorkspaceSetup.Name != oldValues.WorkspaceSetup.Name;
                if (workspaceChanged)
                {
                    mcMod.WorkspaceSetup.Setup(mcMod);
                }

                if (organizationChanged || nameChanged || modidChanged)
                {
                    CodeGenerationService.RegenerateSourceCode(mcMod);
                }
                else
                {
                    bool versionChanged = mcMod.ModInfo.Version != oldValues.ModInfo.Version;
                    bool mcVersionChanged = mcMod.ModInfo.McVersion != oldValues.ModInfo.McVersion;
                    if (versionChanged || mcVersionChanged)
                    {
                        CodeGenerationService.RegenerateScript(CodeGeneration.SourceCodeLocator.Hook(mcMod.ModInfo.Name, mcMod.Organization).ClassName, mcMod);
                    }
                }
                ModHelper.ExportMcInfo(ModInfoSerializer, mcMod.ModInfo);
                ModHelper.ExportMod(ModSerializer, mcMod);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
                return;
            }
            Log.Info($"Changes to {mcMod.ModInfo.Name} saved successfully", true);
        }

        private void ChangeForgeVersion(McMod mcMod)
        {
            string modRoot = ModPaths.ModRootFolder(mcMod.ModInfo.Name);

            // remove every folder except 'src'
            foreach (string directory in Directory.EnumerateDirectories(modRoot))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if (info.Name != "src")
                {
                    FileSystem.DeleteDirectory(directory, true);
                }
            }

            // remove every file except FmgModInfo
            foreach (string file in Directory.EnumerateFiles(modRoot))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name != ModPaths.FmgInfoFileName)
                {
                    FileSystem.DeleteFile(file, true);
                }
            }

            // unzip forgeversion to temp path
            string tempDirPath = Path.Combine(modRoot, "temp");
            Directory.CreateDirectory(tempDirPath);
            mcMod.ForgeVersion.UnZip(tempDirPath);

            FileSystem.DeleteDirectory(Path.Combine(tempDirPath, "src"), false);
            FileSystem.MoveDirectoriesAndFiles(tempDirPath, modRoot);
            FileSystem.DeleteDirectory(tempDirPath, false);
            Log.Info($"Changed forge version for {mcMod.CachedName} to {mcMod.ForgeVersion.Name}");
        }
    }
}
