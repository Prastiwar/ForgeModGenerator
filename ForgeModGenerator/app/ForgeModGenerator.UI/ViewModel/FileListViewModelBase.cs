using ForgeModGenerator.Miscellaneous;
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
using System.Windows;
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
            AllowedExtensions = new string[] { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
        }

        public Visibility EmptyMessageVisibility => SessionContext.IsModSelected ? Visibility.Collapsed : Visibility.Visible;

        public abstract string CollectionRootPath { get; }

        private ObservableCollection<FileCollection> files;
        public ObservableCollection<FileCollection> Files {
            get => files;
            set => Set(ref files, value);
        }

        private FileCollection selectedFiles;
        public FileCollection SelectedFiles {
            get => selectedFiles;
            set => Set(ref selectedFiles, value);
        }

        private ICommand addCommand;
        public ICommand AddCommand => addCommand ?? (addCommand = new RelayCommand<FileCollection>(AddNewFile));

        private ICommand removeCommand;
        public ICommand RemoveCommand => removeCommand ?? (removeCommand = new RelayCommand<Tuple<ObservableCollection<string>, string>>(Remove));

        public string[] AllowedExtensions { get; protected set; }

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && Directory.Exists(CollectionRootPath);

        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Files = FindCollection(CollectionRootPath);
                if (Files.Count <= 0)
                {
                    Files.Add(new FileCollection(CollectionRootPath));
                }
                return true;
            }
            return false;
        }

        protected virtual void Remove(Tuple<ObservableCollection<string>, string> param)
        {
            if (param == null)
            {
                Log.Warning("Remove item called with null parameter");
                return;
            }
            else if (param.Item1 == null)
            {
                Log.Warning("Remove item called with null collection", true);
                return;
            }

            if (param.Item1.Remove(param.Item2))
            {
                try
                {
                    FileSystem.DeleteFile(param.Item2, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Couldn't delete {param.Item2}. Make sure it's not used by any process", true);
                    param.Item1.Add(param.Item2); // delete failed, so get item back to collection
                    return;
                }
            }
        }

        protected virtual void AddNewFile(FileCollection collection)
        {
            if (collection == null || collection.Paths == null)
            {
                Log.Warning("Can't add new file to null collection", true);
                return;
            }
            DialogResult result = OpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach (string filePath in OpenFileDialog.FileNames)
                {
                    string fileName = new FileInfo(filePath).Name;
                    string newFilePath = Path.Combine(collection.DestinationPath, fileName);
                    try
                    {
                        File.Copy(filePath, newFilePath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't add {fileName} to {collection.HeaderName}. Make sure the file is not opened by any process.", true);
                        continue;
                    }
                    collection.Paths.Add(newFilePath);
                }
            }
        }

        protected virtual ObservableCollection<FileCollection> FindCollection(string path)
        {
            List<FileCollection> initCollection = new List<FileCollection>(10);

            AddFilesToCollection(path);
            foreach (string directory in Directory.EnumerateDirectories(path, "*", System.IO.SearchOption.AllDirectories))
            {
                AddFilesToCollection(directory);
            }
            void AddFilesToCollection(string directoryPath)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(directoryPath).Where(filePath => AllowedExtensions.Any(ext => ext == Path.GetExtension(filePath)));
                if (files.Any())
                {
                    FileCollection fileCollection = new FileCollection(directoryPath);
                    foreach (string filePath in files)
                    {
                        if (AllowedExtensions.Any(x => x == Path.GetExtension(filePath)))
                        {
                            fileCollection.Paths.Add(Path.GetFullPath(filePath).Replace('\\', '/'));
                        }
                    }
                    initCollection.Add(fileCollection);
                }
            }
            return new ObservableCollection<FileCollection>(initCollection);
        }

        protected virtual void OnSessionContexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                Refresh();
            }
        }
    }
}
