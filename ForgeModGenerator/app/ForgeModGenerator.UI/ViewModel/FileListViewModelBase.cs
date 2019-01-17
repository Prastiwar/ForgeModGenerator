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
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FileListViewModelBase : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }
        protected OpenFileDialog OpenFileDialog { get; }

        public FileListViewModelBase(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            SessionContext.PropertyChanged += OnSessionContexPropertyChaged;
        }

        public abstract string CollectionRootPath { get; }

        private ObservableCollection<FileCollection> files;
        public ObservableCollection<FileCollection> Files {
            get => files;
            set => Set(ref files, value);
        }

        private ICommand addCommand;
        public ICommand AddCommand => addCommand ?? (addCommand = new RelayCommand<FileCollection>(AddNew));

        private ICommand removeCommand;
        public ICommand RemoveCommand => removeCommand ?? (removeCommand = new RelayCommand<Tuple<ObservableCollection<string>, string>>(Remove));

        protected virtual void Refresh()
        {
            if (SessionContext.SelectedMod != null && Directory.Exists(CollectionRootPath))
            {
                Files = FindCollection(CollectionRootPath);
            }
        }

        protected virtual void Remove(Tuple<ObservableCollection<string>, string> param)
        {
            param.Item1.Remove(param.Item2);
            FileSystem.DeleteFile(param.Item2, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        protected virtual void AddNew(FileCollection collection)
        {
            DialogResult result = OpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach (string filePath in OpenFileDialog.FileNames)
                {
                    string fileName = new FileInfo(filePath).Name;
                    string newFilePath = Path.Combine(collection.DestinationPath, fileName);
                    File.Copy(filePath, newFilePath);
                    collection.Paths.Add(newFilePath);
                }
            }
        }

        protected virtual ObservableCollection<FileCollection> FindCollection(string path)
        {
            List<FileCollection> initCollection = new List<FileCollection>(10);
            foreach (string directory in Directory.EnumerateDirectories(path, "*", System.IO.SearchOption.AllDirectories))
            {
                bool hasAnyFile = Directory.EnumerateFiles(directory).Any();
                if (hasAnyFile)
                {
                    FileCollection fileCollection = new FileCollection(directory);
                    foreach (string filePath in Directory.EnumerateFiles(directory))
                    {
                        fileCollection.Paths.Add(filePath);
                    }
                    initCollection.Add(fileCollection);
                }
            }
            return new ObservableCollection<FileCollection>(initCollection);
        }

        protected virtual void OnSessionContexPropertyChaged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                Refresh();
            }
        }
    }
}
