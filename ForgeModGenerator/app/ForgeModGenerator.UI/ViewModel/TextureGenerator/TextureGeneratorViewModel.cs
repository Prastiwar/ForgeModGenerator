using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            Textures = FindAllTextures(ModPaths.Textures(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid));
        }

        private ObservableCollection<TextureCollection> textures;
        public ObservableCollection<TextureCollection> Textures {
            get => textures;
            set => Set(ref textures, value);
        }

        private ICommand addTexture;
        public ICommand AddTexture => addTexture ?? (addTexture = new RelayCommand<TextureCollection>(AddNewTexture));

        private ICommand removeTexture;
        public ICommand RemoveTexture => removeTexture ?? (removeTexture = new RelayCommand<Tuple<ObservableCollection<string>, string>>(RemoveTextureFrom));

        private void RemoveTextureFrom(Tuple<ObservableCollection<string>, string> param)
        {
            param.Item1.Remove(param.Item2);
            FileSystem.DeleteFile(param.Item2, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        private void AddNewTexture(TextureCollection collection)
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
                    string fileName = new FileInfo(filePath).Name;
                    string newFilePath = Path.Combine(collection.DestinationPath, fileName);
                    File.Copy(filePath, newFilePath);
                    collection.Textures.Add(newFilePath);
                }
            }
        }

        private ObservableCollection<TextureCollection> FindAllTextures(string path)
        {
            List<TextureCollection> textures = new List<TextureCollection>();
            foreach (string directory in Directory.EnumerateDirectories(path, "*", System.IO.SearchOption.AllDirectories))
            {
                bool hasAnyFile = Directory.EnumerateFiles(directory).Any();
                if (hasAnyFile)
                {
                    TextureCollection texture = new TextureCollection(directory);
                    foreach (string filePath in Directory.EnumerateFiles(directory))
                    {
                        texture.Textures.Add(filePath);
                    }
                    textures.Add(texture);
                }
            }
            return new ObservableCollection<TextureCollection>(textures);
        }
    }
}
