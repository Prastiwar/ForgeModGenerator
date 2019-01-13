using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.IO.Compression;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }

        public ModGeneratorViewModel(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
        }

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
            // TODO: Safety checks
            string newModPath = Path.Combine(AppPaths.Mods, SelectedEditMod.ModInfo.Name);
            selectedForgeVersion.UnZip(newModPath);

            string javaSource = ModPaths.JavaSource(SelectedEditMod.ModInfo.Name);
            string[] organizationPaths = Directory.GetDirectories(javaSource);
            foreach (string organization in organizationPaths)
            {
                Directory.Delete(organization, true);
            }

            string generatedPath = ModPaths.GeneratedSourceCode(SelectedEditMod.ModInfo.Name, SelectedEditMod.ModInfo.Modid, SelectedEditMod.Organization);
            string assetsPath = ModPaths.Assets(SelectedEditMod.ModInfo.Name, SelectedEditMod.ModInfo.Modid);
            GenerateFolders(assetsPath, "blockstates", "lang", "recipes", "sounds", "models/item", "textures/blocks", "textures/entity", "textures/items", "textures/models/armor");
            ExtractCore(generatedPath);

            SessionContext.Mods.Add(SelectedEditMod);
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
            string tempZipPath = Path.Combine(rootPath, "temp.zip");
            //File.WriteAllBytes(tempZipPath, Properties.Resources.SourceCodeArchive); // TODO: create and add zip to resources
            ZipFile.ExtractToDirectory(tempZipPath, rootPath);
            File.Delete(tempZipPath);
        }
    }
}
