using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.ViewModels
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : GeneratorViewModelBase<McMod>
    {
        public ModGeneratorViewModel(GeneratorContext<McMod> context,
                                     IDialogService dialogService,
                                     IFileSystem fileSystem,
                                     ISerializer<McMod> modSerializer,
                                     ISerializer<McModInfo> modInfoSerializer)
            : base(context)
        {
            DialogService = dialogService;
            FileSystem = fileSystem;
            ModSerializer = modSerializer;
            ModInfoSerializer = modInfoSerializer;
            ResetNewMod();
        }

        private readonly string[] assetsFolerToGenerate = new string[] {
            "blockstates", "lang", "sounds", "models/item",
            "recipes/shapeless", "recipe/shaped", "recipe/smelting",
            "textures/blocks", "textures/entity", "textures/items", "textures/models/armor"
        };

        protected ISerializer<McMod> ModSerializer { get; }
        protected ISerializer<McModInfo> ModInfoSerializer { get; }

        public IDialogService DialogService { get; }
        public IFileSystem FileSystem { get; }

        public IEnumerable<ModSide> Sides => ReflectionHelper.GetEnumValues<ModSide>();

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

        private ICommand showModContainerCommand;
        public ICommand ShowModContainerCommand => showModContainerCommand ?? (showModContainerCommand = new DelegateCommand<McMod>(ShowContainer));

        public override string DirectoryRootPath => AppPaths.Mods;

        public override Task<bool> Refresh() => Task.FromResult(true);

        private void ShowContainer(McMod mcMod) => Process.Start(ModPaths.ModRootFolder(mcMod.ModInfo.Name));

        private string OnModValidate(object sender, string propertyName) => OnValidate(sender, SessionContext.Mods, propertyName);

        protected override async void RemoveItem(McMod item)
        {
            bool shouldRemove = await DialogService.ShowMessage("Are you sure you want to delete this mod? Folder will be moved to trash bin",
                                                                "Confirm deletion", "Yes", "No", null).ConfigureAwait(true);
            if (shouldRemove)
            {
                if (SessionContext.Mods.Remove(item))
                {
                    try
                    {
                        FileSystem.DeleteDirectory(ModPaths.ModRootFolder(item.ModInfo.Name), true);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Couldn't remove mod. Make sure folder or its file is not used in any other process", true);
                        SessionContext.Mods.Add(item); // remove failed, so rollback
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

        protected override void EditItem(McMod item)
        {
            SelectedEditMod = item;
            item.ValidateProperty += OnModValidate;
            EditorForm.OpenItemEditor(item);
        }

        private void CreateNewMod(McMod mcMod)
        {
            IEditorForm<McMod> tempEditor = Context.EditorFormFactory.Create();
            tempEditor.Validator = Context.Validator;
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

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<McMod> e)
        {
            if (e.Result)
            {
                if (e.ActualItem.Validate().IsValid)
                {
                    SaveModChanges(e.CachedItem, e.ActualItem);
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
            ValidateResult validation = Context.Validator.Validate(mcMod);
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
            Context.CodeGenerationService.RegenerateSourceCode(mcMod);

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

        private void SaveModChanges(McMod oldModValues, McMod mcMod)
        {
            ValidateResult validation = Context.Validator.Validate(mcMod);
            if (!validation.IsValid)
            {
                Log.Warning($"Selected mod is not valid. Reason: {validation}", true);
                return;
            }
            try
            {
                bool organizationChanged = mcMod.Organization != oldModValues.Organization;
                bool nameChanged = mcMod.ModInfo.Name != oldModValues.ModInfo.Name;
                bool modidChanged = mcMod.ModInfo.Modid != oldModValues.ModInfo.Modid;

                if (organizationChanged)
                {
                    string oldOrganizationPath = ModPaths.OrganizationRootFolder(oldModValues.ModInfo.Name, oldModValues.Organization);
                    if (!FileSystem.RenameDirectory(oldOrganizationPath, mcMod.Organization))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldOrganizationPath), "Rename failed");
                        mcMod.Organization = oldModValues.Organization;
                    }
                }
                if (modidChanged)
                {
                    string oldAssetPath = ModPaths.AssetsFolder(oldModValues.ModInfo.Name, oldModValues.ModInfo.Modid);
                    if (!FileSystem.RenameDirectory(oldAssetPath, mcMod.ModInfo.Modid))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldAssetPath), "Rename failed");
                        mcMod.ModInfo.Modid = oldModValues.ModInfo.Modid;
                    }
                }
                if (nameChanged)
                {
                    bool canChangeName = true;
                    string oldSourceCodePath = ModPaths.SourceCodeRootFolder(oldModValues.ModInfo.Name, mcMod.Organization);
                    if (!FileSystem.RenameDirectory(oldSourceCodePath, mcMod.ModInfo.Name.ToLower()))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldSourceCodePath), "Rename failed");
                        mcMod.ModInfo.Name = oldModValues.ModInfo.Name;
                    }

                    string oldNamePath = ModPaths.ModRootFolder(oldModValues.ModInfo.Name);
                    if (canChangeName && !FileSystem.RenameDirectory(oldNamePath, mcMod.ModInfo.Name))
                    {
                        DialogService.ShowMessage(StaticMessage.GetOperationFailedMessage(oldNamePath), "Rename failed");
                        string sourceCodeParentPath = new DirectoryInfo(oldSourceCodePath).Parent.FullName;
                        string newSourceCodePath = Path.Combine(sourceCodeParentPath, mcMod.ModInfo.Name.ToLower());
                        FileSystem.RenameDirectory(newSourceCodePath, oldModValues.ModInfo.Name.ToLower());
                        mcMod.ModInfo.Name = oldModValues.ModInfo.Name;
                    }
                }

                bool forgeVersionChanged = mcMod.ForgeVersion.Name != oldModValues.ForgeVersion.Name;
                if (forgeVersionChanged)
                {
                    ChangeForgeVersion(mcMod);
                }

                bool workspaceChanged = mcMod.WorkspaceSetup.Name != oldModValues.WorkspaceSetup.Name;
                if (workspaceChanged)
                {
                    mcMod.WorkspaceSetup.Setup(mcMod);
                }

                if (organizationChanged || nameChanged || modidChanged)
                {
                    Context.CodeGenerationService.RegenerateSourceCode(mcMod);
                }
                else
                {
                    bool versionChanged = mcMod.ModInfo.Version != oldModValues.ModInfo.Version;
                    bool mcVersionChanged = mcMod.ModInfo.McVersion != oldModValues.ModInfo.McVersion;
                    if (versionChanged || mcVersionChanged)
                    {
                        string className = CodeGeneration.SourceCodeLocator.Hook(mcMod.ModInfo.Name, mcMod.Organization).ClassName;
                        Context.CodeGenerationService.GetScriptCodeGenerator(className, mcMod).RegenerateScript();
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

            // structure root path properly
            FileSystem.DeleteDirectory(Path.Combine(tempDirPath, "src"), false);
            FileSystem.MoveDirectoriesAndFiles(tempDirPath, modRoot);
            FileSystem.DeleteDirectory(tempDirPath, false);
            Log.Info($"Changed forge version for {mcMod.ModInfo.Name} to {mcMod.ForgeVersion.Name}");
        }

        protected override void RegenerateCode() => throw new NotImplementedException();
    }
}
