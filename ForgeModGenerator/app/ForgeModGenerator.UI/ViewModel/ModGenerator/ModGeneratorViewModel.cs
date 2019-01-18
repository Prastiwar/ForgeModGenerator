using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }
        public ModValidationService Validator { get; }
        public IWorkspaceSetupService WorkspaceService { get; }

        public ModGeneratorViewModel(ISessionContextService sessionContext, ModValidationService validator, IWorkspaceSetupService workspaceService)
        {
            SessionContext = sessionContext;
            Validator = validator;
            WorkspaceService = workspaceService;

            if (IsInDesignMode || IsInDesignModeStatic)
            {
                SessionContextService context = sessionContext as SessionContextService;
                context.Mods = new ObservableCollection<Mod>() {
                    new Mod(new McModInfo(){ Name = "ExampleMod", Modid = "examplemod" }, "exampleorg", SessionContext.ForgeVersions[0]),
                    new Mod(new McModInfo(){ Name = "ExampleModTwo", Modid = "examplemodtwo" }, "exampleorg2", SessionContext.ForgeVersions[0]),
                    new Mod(new McModInfo(){ Name = "ExampleModThree", Modid = "examplemodthree" }, "exampleorg3", SessionContext.ForgeVersions[0])
                };
            }
            NewMod = new Mod(new McModInfo() {
                Name = "NewExampleMod",
                Modid = "newexamplemod",
                Credits = "For someone",
                Description = "Some description",
                McVersion = "12.2",
                Version = "1.0",
                AuthorList = new ObservableCollection<string>() { "Me", "And Him" },
                Dependencies = new ObservableCollection<string>() { "OtherMod", "EvenOtherMod" },
                Screenshots = new ObservableCollection<string>() { "url", "otherurl" }
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

        private ICommand addNewForgeVersion;
        public ICommand AddNewForgeVersion => addNewForgeVersion ?? (addNewForgeVersion = new RelayCommand(AddNewForge));

        private ICommand createMod;
        public ICommand CreateMod => createMod ?? (createMod = new RelayCommand(() => GenerateMod(NewMod)));

        private ICommand addNewItem;
        public ICommand AddNewItem => addNewItem ?? (addNewItem = new RelayCommand<ObservableCollection<string>>(AddNewItemTo));

        private ICommand removeItem;
        public ICommand RemoveItem => removeItem ?? (removeItem = new RelayCommand<Tuple<ObservableCollection<string>, string>>(RemoveItemFromList));

        private ICommand saveChanges;
        public ICommand SaveChanges => saveChanges ?? (saveChanges = new RelayCommand<Mod>(SaveModChanges));

        private void AddNewItemTo(ObservableCollection<string> collection) => collection.Add("NewItem");
        private void RemoveItemFromList(Tuple<ObservableCollection<string>, string> param) => param.Item1.Remove(param.Item2);

        private void AddNewForge()
        {
            Process.Start("https://files.minecraftforge.net/"); // to install version
            Process.Start(AppPaths.ForgeVersions); // paste zip there
        }

        private void GenerateMod(Mod mod)
        {
            if (!Validator.IsValid(mod))
            {
                Log.InfoBox($"Selected mod is not valid");
                return;
            }
            string newModPath = Path.Combine(AppPaths.Mods, mod.ModInfo.Name);
            if (Directory.Exists(newModPath))
            {
                Log.InfoBox($"{newModPath} already exists");
                return;
            }
            mod.ForgeVersion.UnZip(newModPath);
            RemoveDumpExample(mod);

            string generatedPath = ModPaths.GeneratedSourceCode(mod.ModInfo.Name, mod.Organization);
            string assetsPath = ModPaths.Assets(mod.ModInfo.Name, mod.ModInfo.Modid);
            GenerateFolders(assetsPath, assetsFolerToGenerate);
            ExtractCore(generatedPath);
            ReplaceTemplateVariables(mod, generatedPath);
            File.WriteAllText(ModPaths.FmgModInfo(mod.ModInfo.Name), JsonConvert.SerializeObject(mod)); // create FmgModInfo file for mod detection

            SessionContext.Mods.Add(mod);
            Log.InfoBox($"{mod.ModInfo.Name} was created successfully");
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
                Directory.CreateDirectory(Path.Combine(rootPath, folder)).Attributes = FileAttributes.ReadOnly;
            }
        }

        private void ExtractCore(string rootPath)
        {
            Directory.CreateDirectory(rootPath);
            string tempZipPath = Path.Combine(rootPath, "temp.zip");
            File.WriteAllBytes(tempZipPath, Properties.Resources.SourceCodeArchive);
            ZipFile.ExtractToDirectory(tempZipPath, rootPath);
            File.Delete(tempZipPath);
        }

        private void ReplaceTemplateVariables(Mod mod, string rootPath)
        {
            string[] allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
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
            Log.InfoBox("?");
        }
    }
}
