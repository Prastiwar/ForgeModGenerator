using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.ModGenerator.SourceCodeGeneration;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.ViewModels
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }
        public IWorkspaceSetupService WorkspaceService { get; }

        public ModGeneratorViewModel(ISessionContextService sessionContext, IWorkspaceSetupService workspaceService)
        {
            SessionContext = sessionContext;
            WorkspaceService = workspaceService;

            NewMod = new Mod(new McModInfo() {
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
        }

        private readonly string[] assetsFolerToGenerate = new string[] {
            "blockstates", "lang", "recipes", "sounds", "models/item", "textures/blocks", "textures/entity", "textures/items", "textures/models/armor"
        };

        public IEnumerable<ModSide> Sides => Enum.GetValues(typeof(ModSide)).Cast<ModSide>();

        private Mod newMod;
        public Mod NewMod {
            get => newMod;
            set => Set(ref newMod, value);
        }

        private Mod selectedEditMod;
        public Mod SelectedEditMod {
            get => selectedEditMod;
            set => Set(ref selectedEditMod, value);
        }

        private ICommand addNewForgeVersionCommand;
        public ICommand AddNewForgeVersionCommand => addNewForgeVersionCommand ?? (addNewForgeVersionCommand = new RelayCommand(AddNewForge));

        private ICommand createModCommand;
        public ICommand CreateModCommand => createModCommand ?? (createModCommand = new RelayCommand(() => GenerateMod(NewMod)));

        private ICommand saveChangesCommand;
        public ICommand SaveChangesCommand => saveChangesCommand ?? (saveChangesCommand = new RelayCommand<Mod>(SaveModChanges));

        private void AddNewForge()
        {
            Process.Start("https://files.minecraftforge.net/"); // to install version
            Process.Start(AppPaths.ForgeVersions); // paste zip there
        }

        private void GenerateMod(Mod mod)
        {
            ValidationResult validation = mod.IsValid();
            if (!validation.IsValid)
            {
                Log.Warning($"Selected mod is not valid. Reason: {validation.ErrorContent}", true);
                return;
            }
            string newModPath = ModPaths.ModRootFolder(mod.ModInfo.Name);
            if (Directory.Exists(newModPath))
            {
                Log.Warning($"{newModPath} already exists", true);
                return;
            }
            mod.ForgeVersion.UnZip(newModPath);
            RemoveDumpExample(mod);

            string assetsPath = ModPaths.AssetsFolder(mod.ModInfo.Name, mod.ModInfo.Modid);
            IOHelper.GenerateFolders(assetsPath, assetsFolerToGenerate);
            RegenerateSourceCode(mod);

            mod.WorkspaceSetup.Setup(mod);
            Mod.Export(mod);
            McModInfo.Export(mod.ModInfo);

            SessionContext.Mods.Add(mod);
            Log.Info($"{mod.ModInfo.Name} was created successfully", true);
        }

        private void RegenerateSourceCode(Mod mod)
        {
            foreach (ScriptCodeGenerator generator in ReflectionHelper.GetSubclasses<ScriptCodeGenerator>(mod))
            {
                generator.RegenerateScript();
            }
        }

        private void RemoveDumpExample(Mod mod)
        {
            string javaSource = ModPaths.JavaSourceFolder(mod.ModInfo.Name);
            string[] organizationPaths = Directory.GetDirectories(javaSource);
            foreach (string organization in organizationPaths)
            {
                Directory.Delete(organization, true);
            }
        }

        private void SaveModChanges(Mod mod)
        {
            ValidationResult validation = mod.IsValid();
            if (!validation.IsValid)
            {
                Log.Warning($"Selected mod is not valid. Reason: {validation.ErrorContent}", true);
                return;
            }
            Mod oldValues = Mod.Import(ModPaths.ModRootFolder(mod.CachedName));
            try
            {
                bool organizationChanged = mod.Organization != oldValues.Organization;
                bool nameChanged = mod.ModInfo.Name != oldValues.ModInfo.Name;
                bool modidChanged = mod.ModInfo.Modid != oldValues.ModInfo.Modid;
                if (organizationChanged || nameChanged || modidChanged)
                {
                    RegenerateSourceCode(mod);
                }
                else
                {
                    bool versionChanged = mod.ModInfo.Version != oldValues.ModInfo.Version;
                    bool mcVersionChanged = mod.ModInfo.McVersion != oldValues.ModInfo.McVersion;
                    if (versionChanged || mcVersionChanged)
                    {
                        new ModHookCodeGenerator(mod).RegenerateScript();
                    }
                }

                bool forgeVersionChanged = mod.ForgeVersion.Name != oldValues.ForgeVersion.Name;
                if (forgeVersionChanged)
                {
                    ChangeForgeVersion(mod);
                }

                bool workspaceChanged = mod.WorkspaceSetup.Name != oldValues.WorkspaceSetup.Name;
                if (workspaceChanged)
                {
                    mod.WorkspaceSetup.Setup(mod);
                }

                McModInfo.Export(mod.ModInfo);
                Mod.Export(mod);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
                return;
            }

            Log.Info($"Changes to {mod.ModInfo.Name} saved successfully", true);
        }

        private void ChangeForgeVersion(Mod mod)
        {
            string modRoot = ModPaths.ModRootFolder(mod.ModInfo.Name);

            // remove every folder except 'src'
            foreach (string directory in Directory.EnumerateDirectories(modRoot))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if (info.Name != "src")
                {
                    IOHelper.DeleteDirectoryToBin(directory);
                }
            }

            // remove every file except FmgModInfo
            foreach (string file in Directory.EnumerateFiles(modRoot))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name != ModPaths.FmgInfoFileName)
                {
                    IOHelper.DeleteFileToBin(file);
                }
            }

            // unzip forgeversion to temp path
            string tempDirPath = Path.Combine(modRoot, "temp");
            Directory.CreateDirectory(tempDirPath);
            mod.ForgeVersion.UnZip(tempDirPath);

            Directory.Delete(Path.Combine(tempDirPath, "src"), true);
            IOHelper.MoveDirectoriesAndFiles(tempDirPath, modRoot);
            Directory.Delete(tempDirPath);
            Log.Info($"Changed forge version for {mod.CachedName} to {mod.ForgeVersion.Name}");
        }
    }
}
