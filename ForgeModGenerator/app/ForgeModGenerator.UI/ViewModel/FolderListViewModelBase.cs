using ForgeModGenerator.Converter;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FolderListViewModelBase<TFolder, TFile> : ViewModelBase
        where TFolder : IFileFolder<TFile>
        where TFile : IFileItem
    {
        public FolderListViewModelBase(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            RemoveMenuItemConverter = new TupleValueConverter<TFolder, TFile>();
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            AllowedExtensions = new string[] { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
        }

        public abstract string FoldersRootPath { get; }

        public ISessionContextService SessionContext { get; }

        public IMultiValueConverter RemoveMenuItemConverter { get; }

        public string[] AllowedExtensions { get; protected set; }

        protected OpenFileDialog OpenFileDialog { get; }

        protected FrameworkElement FileEditForm { get; set; }

        protected string EditFileCacheKey => "EditFileCacheKey";

        private ObservableCollection<TFolder> folders;
        public ObservableCollection<TFolder> Folders {
            get => folders;
            set => Set(ref folders, value);
        }

        private TFolder selectedFolder;
        public TFolder SelectedFolder {
            get => selectedFolder;
            set => Set(ref selectedFolder, value);
        }

        private TFile selectedFile;
        public TFile SelectedFile {
            get => selectedFile;
            set => Set(ref selectedFile, value);
        }

        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<TFile>(OpenFileEditor));

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new RelayCommand<TFolder>(AddNewFileToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new RelayCommand<TFolder>(RemoveFolder));

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersRootPath));
        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Folders = FindFolders(FoldersRootPath, true);
                return true;
            }
            return false;
        }

        protected virtual bool CanCloseFileEditor(bool result, TFile fileBeforeEdit, TFile fileAfterEdit) => true;
        protected virtual void OnFileEditorClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            bool result = (bool)eventArgs.Parameter;
            if (!CanCloseFileEditor(result, (TFile)MemoryCache.Default.Get(EditFileCacheKey), SelectedFile))
            {
                eventArgs.Cancel();
            }
        }

        protected virtual void OnFileEditorOpening(object sender, DialogOpenedEventArgs eventArgs) { }
        protected virtual async void OpenFileEditor(TFile file)
        {
            SelectedFile = file;
            bool result = false;
            try
            {
                MemoryCache.Default.Set(EditFileCacheKey, SelectedFile.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration);
                FileEditForm.DataContext = SelectedFile;
                result = (bool)await DialogHost.Show(FileEditForm, OnFileEditorOpening, OnFileEditorClosing);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't open edit form for {file.Info.Name}", true);
                return;
            }
            OnFileEdited(result, file);
        }

        protected virtual void OnFileEdited(bool result, TFile file)
        {
            if (!result)
            {
                // TODO: Undo commands
                file.CopyValues(MemoryCache.Default.Remove(EditFileCacheKey));
            }
        }

        protected void RemoveFolder(TFolder folder)
        {
            try
            {
                folder.Delete();
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected virtual void RemoveFileFromFolder(Tuple<TFolder, TFile> param)
        {
            try
            {
                param.Item1.Remove(param.Item2);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected virtual void AddNewFileToFolder(TFolder folder)
        {
            try
            {
                DialogResult dialogResult = OpenFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    foreach (string filePath in OpenFileDialog.FileNames)
                    {
                        folder.Add(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected virtual ObservableCollection<TFolder> FindFolders(string path, bool createRootIfEmpty = false)
        {
            ObservableCollection<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = FindFoldersFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = DeserializeCollectionFromFile(path);
            }

            bool isEmpty = found == null || found.Count <= 0;
            if (isEmpty && createRootIfEmpty)
            {
                found = CreateEmptyFoldersRoot(path);
            }
            return found;
        }

        protected ObservableCollection<TFolder> CreateEmptyFoldersRoot(string path) => new ObservableCollection<TFolder>() { Util.CreateInstance<TFolder>(path) };

        protected ObservableCollection<TFolder> DeserializeCollectionFromFile(string path)
        {
            string json = File.ReadAllText(path);
            try
            {
                return JsonConvert.DeserializeObject<ObservableCollection<TFolder>>(json);
            }
            catch (Exception ex)
            {
                Type loadType = typeof(ObservableCollection<TFolder>);
                Log.Error(ex, $"Couldnt load {loadType} from {json}");
            }
            return null;
        }

        protected ObservableCollection<TFolder> FindFoldersFromDirectory(string path)
        {
            List<TFolder> initCollection = new List<TFolder>(10);
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
                    TFolder folder = Util.CreateInstance<TFolder>(directoryPath);
                    foreach (string filePath in files)
                    {
                        if (AllowedExtensions.Any(x => x == Path.GetExtension(filePath)))
                        {
                            folder.Add(Path.GetFullPath(filePath).Replace('\\', '/'));
                        }
                    }
                    initCollection.Add(folder);
                }
            }
            return new ObservableCollection<TFolder>(initCollection);
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
