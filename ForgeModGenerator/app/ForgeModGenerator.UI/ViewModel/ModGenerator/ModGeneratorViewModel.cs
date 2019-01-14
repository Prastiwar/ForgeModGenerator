using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }
        public ModValidationService Validator { get; }

        public ModGeneratorViewModel(ISessionContextService sessionContext, ModValidationService validator)
        {
            Validator = validator;
            SessionContext = sessionContext;
        }

        private readonly string[] assetsFolerToGenerate = new string[] {
            "blockstates", "lang", "recipes", "sounds", "models/item", "textures/blocks", "textures/entity", "textures/items", "textures/models/armor"
        };

        private Mod selectedEditMod;
        public Mod SelectedEditMod {
            get => selectedEditMod;
            set => Set(ref selectedEditMod, value);
        }

        private ForgeVersion selectedForgeVersion;
        public ForgeVersion SelectedForgeVersion {
            get => selectedForgeVersion;
            set => Set(ref selectedForgeVersion, value);
        }

        private ICommand createMod;
        public ICommand CreateMod { get => createMod ?? (createMod = new RelayCommand(GenerateMod)); }

        private void GenerateMod()
        {
            SelectedEditMod = new Mod(new McModInfo() {
                Name = "ExampleTestMod",
                Modid = "exampletestmod"
            }, "testorg");
            SelectedForgeVersion = SessionContext.ForgeVersions[0];
            if (!Validator.Validate(SelectedEditMod))
            {
                Log.InfoBox($"Selected mod is not valid");
                return;
            }
            string newModPath = Path.Combine(AppPaths.Mods, SelectedEditMod.ModInfo.Name);
            if (Directory.Exists(newModPath))
            {
                Log.InfoBox($"{newModPath} already exists");
                return;
            }
            selectedForgeVersion.UnZip(newModPath);
            RemoveDumpExample();

            string generatedPath = ModPaths.GeneratedSourceCode(SelectedEditMod.ModInfo.Name, SelectedEditMod.Organization);
            string assetsPath = ModPaths.Assets(SelectedEditMod.ModInfo.Name, SelectedEditMod.ModInfo.Modid);
            GenerateFolders(assetsPath, assetsFolerToGenerate);
            ExtractCore(generatedPath);
            File.WriteAllText(ModPaths.FmgModInfo(SelectedEditMod.ModInfo.Name), JsonConvert.SerializeObject(SelectedEditMod)); // create FmgModInfo file for mod detection

            SessionContext.Mods.Add(SelectedEditMod);
        }

        private void RemoveDumpExample()
        {
            string javaSource = ModPaths.JavaSource(SelectedEditMod.ModInfo.Name);
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
            ReplaceTemplateVariables(rootPath);
        }

        private void ReplaceTemplateVariables(string rootPath)
        {
            string modidKey = "{modid}";
            string modnameKey = "{modname}";
            string modnameLowerKey = "{modnamelower}";
            string modVersionKey = "{modversion}";
            string mcVersionKey = "{mcversions}";
            string organizationKey = "{organization}";
            string[] allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
            {
                string content = File.ReadAllText(file);
                string newContent = content.Replace(modnameKey, SelectedEditMod.ModInfo.Name)
                                            .Replace(modnameLowerKey, SelectedEditMod.ModInfo.Name.ToLower())
                                            .Replace(organizationKey, SelectedEditMod.Organization)
                                            .Replace(modVersionKey, SelectedEditMod.ModInfo.Version)
                                            .Replace(mcVersionKey, SelectedEditMod.ModInfo.McVersion)
                                            .Replace(modidKey, SelectedEditMod.ModInfo.Modid);
                File.WriteAllText(file, newContent);

                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains(modnameKey))
                {
                    File.Move(file, file.Replace(modnameKey, SelectedEditMod.ModInfo.Name));
                }
            }
        }
    }
}
