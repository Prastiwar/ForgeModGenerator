using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }

        public TextureGeneratorViewModel(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            if (SessionContext.SelectedMod == null)
            {
                Log.InfoBox("Select mod first");
                return;
            }
            Blocks = FindTextures(ModPaths.TexturesBlocks(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
            Entity = FindTextures(ModPaths.TexturesEntity(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
            Items = FindTextures(ModPaths.TexturesItems(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
            Armor = FindTextures(ModPaths.TexturesModelsArmor(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
        }

        private ObservableCollection<string> blocks;
        public ObservableCollection<string> Blocks {
            get => blocks;
            set => Set(ref blocks, value);
        }

        private ObservableCollection<string> entity;
        public ObservableCollection<string> Entity {
            get => entity;
            set => Set(ref entity, value);
        }

        private ObservableCollection<string> items;
        public ObservableCollection<string> Items {
            get => items;
            set => Set(ref items, value);
        }

        private ObservableCollection<string> armor;
        public ObservableCollection<string> Armor {
            get => armor;
            set => Set(ref armor, value);
        }

        private ICommand addTexture;
        public ICommand AddTexture { get => addTexture ?? (addTexture = new RelayCommand<ObservableCollection<string>>(AddNewTexture)); }

        private void AddNewTexture(ObservableCollection<string> collection)
        {
            OpenFileDialog dialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true,
                Filter = "Image (*.png) | *.png"
            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach (string filePath in dialog.FileNames)
                {
                    // TODO: Add texture
                }
            }
        }

        private ObservableCollection<string> FindTextures(string path)
        {
            Mod mod = SessionContext.SelectedMod;
            string[] texturePaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            List<string> foundTextures = new List<string>(texturePaths.Length);
            foreach (string texturePath in texturePaths)
            {
                foundTextures.Add(texturePath);
            }
            return new ObservableCollection<string>(foundTextures);
        }
    }
}
