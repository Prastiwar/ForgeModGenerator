using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeModGenerator
{
    public class FileEditor<TFolder, TFile>
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public class FileEditedEventArgs : EventArgs
        {
            public FileEditedEventArgs(bool result, TFolder folder, TFile cachedFile, TFile actualFile)
            {
                Result = result;
                Folder = folder;
                CachedFile = cachedFile;
                ActualFile = actualFile;
            }
            public bool Result { get; }
            public TFolder Folder { get; }
            public TFile CachedFile { get; }
            public TFile ActualFile { get; }
        }

        public class FileEditorClosingDialogEventArgs : EventArgs
        {
            public FileEditorClosingDialogEventArgs(DialogClosingEventArgs dialogClosingArgs, TFolder folder, TFile file)
            {
                Folder = folder;
                File = file;
                DialogClosingArgs = dialogClosingArgs;
            }
            public DialogClosingEventArgs DialogClosingArgs { get; }
            public TFolder Folder { get; }
            public TFile File { get; }
        }

        public class FileEditorOpeningDialogEventArgs : EventArgs
        {
            public FileEditorOpeningDialogEventArgs(DialogOpenedEventArgs dialogOpenedEventArgs, TFolder folder, TFile file)
            {
                Folder = folder;
                File = file;
                DialogOpenedArgs = dialogOpenedEventArgs;
            }
            public DialogOpenedEventArgs DialogOpenedArgs { get; }
            public TFolder Folder { get; }
            public TFile File { get; }
        }

        public FileEditor(IDialogService dialogService, FrameworkElement fileEditForm)
        {
            DialogService = dialogService;
            FileEditForm = fileEditForm;
        }

        private event EventHandler<FileEditedEventArgs> OnFileEditedHandler;
        public event EventHandler<FileEditedEventArgs> OnFileEdited {
            add => OnFileEditedHandler += value;
            remove => OnFileEditedHandler -= value;
        }

        public FrameworkElement FileEditForm { get; set; }

        public TFile EditingFile { get; protected set; }

        protected IDialogService DialogService { get; set; }

        protected string EditFileCacheKey => "EditFileCacheKey";

        public virtual async void OpenFileEditor(Tuple<TFolder, TFile> param)
        {
            EditingFile = param.Item2;
            if (FileEditForm == null)
            {
                return;
            }
            MemoryCache.Default.Set(EditFileCacheKey, EditingFile.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration); // cache file state so it can be restored later
            bool result = false;
            try
            {
                result = (bool)await DialogHost.Show(FileEditForm,
                    (sender, args) => { OnFileEditorOpening(sender, new FileEditorOpeningDialogEventArgs(args, param.Item1, param.Item2)); },
                    (sender, args) => { OnFileEditorClosing(sender, new FileEditorClosingDialogEventArgs(args, param.Item1, param.Item2)); });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't open edit form for {param.Item2.Info.Name}", true);
                return;
            }
            if (OnFileEditedHandler == null)
            {
                OnFileEditedHandler = DefaultOnFileEdited;
            }
            OnFileEditedHandler(this, new FileEditedEventArgs(result, param.Item1, (TFile)MemoryCache.Default.Remove(EditFileCacheKey), param.Item2));
        }

        protected virtual async Task<bool> CanCloseFileEditor(FileEditedEventArgs args)
        {
            if (!args.Result)
            {
                if (args.ActualFile.IsDirty)
                {
                    return await DialogService.ShowMessage("Are you sure you want to exit form? Changes won't be saved", "Unsaved changes", "Yes", "No", null);
                }
            }
            return true;
        }

        protected virtual async void OnFileEditorOpening(object sender, FileEditorOpeningDialogEventArgs args) => FileEditForm.DataContext = EditingFile;

        protected async void OnFileEditorClosing(object sender, FileEditorClosingDialogEventArgs args)
        {
            bool result = (bool)args.DialogClosingArgs.Parameter;
            bool canClose = await CanCloseFileEditor(new FileEditedEventArgs(result, args.Folder, (TFile)MemoryCache.Default.Get(EditFileCacheKey), args.File));
            if (!canClose)
            {
                args.DialogClosingArgs.Cancel();
            }
        }

        protected virtual void DefaultOnFileEdited(object sender, FileEditedEventArgs args)
        {
            if (!args.Result)
            {
                args.ActualFile.CopyValues(args.CachedFile);
            }
        }
    }
}
