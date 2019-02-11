using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            string newModPath = ModPaths.ModRoot(mod.ModInfo.Name);
            if (Directory.Exists(newModPath))
            {
                Log.Warning($"{newModPath} already exists", true);
                return;
            }
            mod.ForgeVersion.UnZip(newModPath);
            RemoveDumpExample(mod);

            string generatedPath = ModPaths.GeneratedSourceCodeFolder(mod.ModInfo.Name, mod.Organization);
            string assetsPath = ModPaths.Assets(mod.ModInfo.Name, mod.ModInfo.Modid);
            GenerateFolders(assetsPath, assetsFolerToGenerate);
            Directory.CreateDirectory(generatedPath);
            ExtractCore(generatedPath);
            ReplaceTemplateVariables(mod, generatedPath);

            mod.WorkspaceSetup.Setup(mod);
            Mod.Export(mod);
            McModInfo.Export(mod.ModInfo);

            SessionContext.Mods.Add(mod);
            Log.Info($"{mod.ModInfo.Name} was created successfully", true);
        }

        private void RemoveDumpExample(Mod mod)
        {
            string javaSource = ModPaths.JavaSource(mod.ModInfo.Name);
            string[] organizationPaths = Directory.GetDirectories(javaSource);
            foreach (string organization in organizationPaths)
            {
                Directory.Delete(organization, true);
            }
        }

        private void GenerateFolders(string rootPath, params string[] generatedFolders)
        {
            Directory.CreateDirectory(rootPath);
            foreach (string folder in generatedFolders)
            {
                Directory.CreateDirectory(Path.Combine(rootPath, folder));
            }
        }

        private void ExtractCore(string rootPath)
        {
            string tempZipPath = Path.Combine(rootPath, "temp.zip");
            File.WriteAllBytes(tempZipPath, Properties.Resources.SourceCodeArchive);
            ZipFile.ExtractToDirectory(tempZipPath, rootPath);
            File.Delete(tempZipPath);
        }

        private void ReplaceTemplateVariables(Mod mod, string rootPath)
        {
            foreach (string file in IOExtensions.EnumerateAllFiles(rootPath))
            {
                string content = File.ReadAllText(file);
                string newContent = content.Replace(CoreSourceCodeVariables.Modname, mod.ModInfo.Name)
                                            .Replace(CoreSourceCodeVariables.ModnameLower, mod.ModInfo.Name.ToLower())
                                            .Replace(CoreSourceCodeVariables.Organization, mod.Organization)
                                            .Replace(CoreSourceCodeVariables.ModVersion, mod.ModInfo.Version)
                                            .Replace(CoreSourceCodeVariables.McVersion, mod.ModInfo.McVersion)
                                            .Replace(CoreSourceCodeVariables.Modid, mod.ModInfo.Modid);
                File.WriteAllText(file, newContent);

                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains(CoreSourceCodeVariables.Modname))
                {
                    File.Move(file, file.Replace(CoreSourceCodeVariables.Modname, mod.ModInfo.Name));
                }
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
            Mod oldValues = Mod.Import(ModPaths.ModRoot(mod.CachedName));
            try
            {
                if (mod.Organization != oldValues.Organization)
                {
                    ChangeOrganization(mod, oldValues);
                }

                if (mod.ModInfo.Name != oldValues.ModInfo.Name)
                {
                    ChangeModName(mod, oldValues);
                }

                if (mod.ModInfo.Modid != oldValues.ModInfo.Modid)
                {
                    ChangeModid(mod, oldValues);
                }

                string hookPath = ModPaths.GeneratedModHookFile(mod.ModInfo.Name, mod.Organization);

                if (mod.ModInfo.Version != oldValues.ModInfo.Version)
                {
                    CodeOperation.ReplaceStringVariableValue(hookPath, oldValues.ModInfo.Version, mod.ModInfo.Version);
                }

                if (mod.ModInfo.McVersion != oldValues.ModInfo.McVersion)
                {
                    CodeOperation.ReplaceStringVariableValue(hookPath, oldValues.ModInfo.McVersion, mod.ModInfo.McVersion);
                }

                if (mod.ForgeVersion.Name != oldValues.ForgeVersion.Name)
                {
                    ChangeForgeVersion(mod);
                }

                if (mod.WorkspaceSetup.Name != oldValues.WorkspaceSetup.Name)
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
            string modRoot = ModPaths.ModRoot(mod.ModInfo.Name);

            // remove every folder except 'src'
            foreach (string directory in Directory.EnumerateDirectories(modRoot))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if (info.Name != "src")
                {
                    IOExtensions.DeleteDirectoryToBin(directory);
                }
            }

            // remove every file except FmgModInfo
            foreach (string file in Directory.EnumerateFiles(modRoot))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name != ModPaths.FmgInfoFileName)
                {
                    IOExtensions.DeleteFileToBin(file);
                }
            }

            // unzip forgeversion to temp path
            string tempDirPath = Path.Combine(modRoot, "temp");
            Directory.CreateDirectory(tempDirPath);
            mod.ForgeVersion.UnZip(tempDirPath);

            Directory.Delete(Path.Combine(tempDirPath, "src"), true);
            IOExtensions.MoveDirectoriesAndFiles(tempDirPath, modRoot);
            Directory.Delete(tempDirPath);
            Log.Info($"Changed forge version for {mod.CachedName} to {mod.ForgeVersion.Name}");
        }

        private void ChangeModid(Mod mod, Mod oldValues)
        {
            string hookPath = ModPaths.GeneratedModHookFile(mod.ModInfo.Name, mod.Organization);
            CodeOperation.ReplaceStringVariableValue(hookPath, oldValues.ModInfo.Modid, mod.ModInfo.Modid);

            string assetsPath = ModPaths.Assets(mod.ModInfo.Name, oldValues.ModInfo.Modid);
            string newAssetsPath = ModPaths.Assets(mod.ModInfo.Name, mod.ModInfo.Modid);
            Directory.Move(assetsPath, newAssetsPath);

            foreach (string file in IOExtensions.EnumerateAllFiles(newAssetsPath))
            {
                CodeOperation.ReplaceStringValue(file, oldValues.ModInfo.Modid, mod.ModInfo.Modid);
            }
            Log.Info($"Changed modid for {mod.CachedName} from {oldValues.ModInfo.Modid} to {mod.ModInfo.Modid}");
        }

        private void ChangeModName(Mod mod, Mod oldValues)
        {
            string oldRootPath = ModPaths.ModRoot(oldValues.ModInfo.Name);
            string newRootPath = ModPaths.ModRoot(mod.ModInfo.Name);

            string oldSourceNamePath = Path.Combine(ModPaths.JavaSource(mod.ModInfo.Name), mod.Organization, oldValues.ModInfo.Name.ToLower());
            string newSourceNamePath = ModPaths.SourceCodeRoot(mod.ModInfo.Name, mod.Organization);
            if (Directory.Exists(newSourceNamePath))
            {
                Log.Warning($"Can't change mod name, {newSourceNamePath} already exists", true);
                return;
            }
            Directory.Move(oldRootPath, newRootPath);
            Directory.Move(oldSourceNamePath, newSourceNamePath);
            string hookPath = ModPaths.GeneratedModHookFile(mod.ModInfo.Name, mod.Organization).Replace('\\', '/');
            foreach (string file in IOExtensions.EnumerateAllFiles(newSourceNamePath))
            {
                string correctFilePath = file;
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains(oldValues.ModInfo.Name))
                {
                    correctFilePath = Path.Combine(file.Substring(0, file.IndexOf(fileInfo.Name)),
                        fileInfo.Name.Replace(oldValues.ModInfo.Name, mod.ModInfo.Name)
                    );
                    File.Move(file, correctFilePath);
                }

                string content = File.ReadAllText(correctFilePath);
                string newContent = null;
                if (correctFilePath.Replace('\\', '/') != hookPath)
                {
                    newContent = content.Replace(oldValues.ModInfo.Name, mod.ModInfo.Name)
                                        .Replace(oldValues.ModInfo.Name.ToLower(), mod.ModInfo.Name.ToLower());
                }
                else
                {
                    // replace correct modname.ToLower() since moid can be equal to modname.ToLower()
                    newContent = content.Replace(oldValues.ModInfo.Name, mod.ModInfo.Name)
                                        .Replace($".{oldValues.ModInfo.Name.ToLower()}", $".{mod.ModInfo.Name.ToLower()}");
                }
                File.WriteAllText(correctFilePath, newContent);
            }
            Log.Info($"Changed name for {mod.CachedName} from {oldValues.ModInfo.Name} to {mod.ModInfo.Name}");
        }

        private void ChangeOrganization(Mod mod, Mod oldValues)
        {
            string oldOrgPath = ModPaths.OrganizationRoot(oldValues.ModInfo.Name, oldValues.Organization);
            string newOrgPath = ModPaths.OrganizationRoot(mod.ModInfo.Name, mod.Organization);
            Directory.Move(oldOrgPath, newOrgPath);
            foreach (string file in IOExtensions.EnumerateAllFiles(newOrgPath))
            {
                CodeOperation.ReplaceStringValue(file, oldValues.Organization, mod.Organization);
            }
            Log.Info($"Changed organization for {mod.CachedName} from {oldValues.Organization} to {mod.Organization}");
        }
    }
}
