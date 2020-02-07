using ForgeModGenerator.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Abstract class with functionality to synchronize file explorer and models </summary>
    public abstract class ExplorerSyncInitViewModelBase<TModel> : SimpleInitViewModelBase<TModel>
        where TModel : ObservableModel
    {
        public ExplorerSyncInitViewModelBase(GeneratorContext<TModel> context, ISynchronizeInvoke synchronizingObject)
            : base(context) => this.synchronizingObject = synchronizingObject;

        private readonly ISynchronizeInvoke synchronizingObject;

        protected IFolderSynchronizer Synchronizer { get; private set; }

        private string fileSearchPatterns;
        protected string FileSearchPatterns {
            get => fileSearchPatterns;
            set => fileSearchPatterns = string.IsNullOrEmpty(value) ? "*" : value;
        }

        public bool IgnoreUserPermission { get; protected set; }

        protected bool AskForUserPermission(string question)
            => IgnoreUserPermission ? true : Context.DialogService.ShowMessage(question, "Decision", "Apply", "Cancel", null).Result;

        /// <summary> Creates Synchronizer instance if it's not already initialized or was disposed </summary>
        protected void IntializeSynchronizer()
        {
            if (Synchronizer == null)
            {
                Synchronizer = new FolderSynchronizer(synchronizingObject, DirectoryRootPath, FileSearchPatterns) {
                    SyncFilter = NotifyFilter.File,
                    DisableSyncWhileSyncing = true
                };
                Synchronizer.SyncChangedFile += OnSyncChangedFile;
                Synchronizer.SyncCreatedFile += OnSyncCreatedFile;
                Synchronizer.SyncRemovedFile += OnSyncRemovedFile;
                Synchronizer.SyncRenamedFile += OnSyncRenamedFile;
            }
        }

        public override async Task<bool> Refresh()
        {
            bool couldRefresh = await base.Refresh().ConfigureAwait(false);
            if (couldRefresh)
            {
                IntializeSynchronizer();
                Synchronizer.SetEnableSynchronization(true);
                return true;
            }
            return false;
        }

        protected override void OnUnloaded()
        {
            Synchronizer.SetEnableSynchronization(false);
            Synchronizer.SyncChangedFile -= OnSyncChangedFile;
            Synchronizer.SyncCreatedFile -= OnSyncCreatedFile;
            Synchronizer.SyncRemovedFile -= OnSyncRemovedFile;
            Synchronizer.SyncRenamedFile -= OnSyncRenamedFile;
            Synchronizer.Dispose();
            Synchronizer = null;
        }

        protected virtual void OnSyncRenamedFile(bool isDirectory, string oldPath, string newPath)
        {
            if (TryGetModelByPath(oldPath, out TModel oldModel))
            {
                if (AskForUserPermission($"Renamed file detected, should I update the model {oldModel.Name}?"))
                {
                    SyncRenamedFile(oldModel, newPath);
                }
            }
        }

        protected virtual void OnSyncRemovedFile(bool isDirectory, string path)
        {
            if (TryGetModelByPath(path, out TModel model))
            {
                if (AskForUserPermission($"Deleted file detected, should I remove model {model.Name}?"))
                {
                    SyncRemovedFile(model);
                }
            }
        }

        protected virtual void OnSyncCreatedFile(bool isDirectory, string path)
        {
            if (AskForUserPermission($"New file detected, should I parse it and add new model?"))
            {
                SyncCreatedFile(isDirectory, path);
            }
        }

        protected virtual void OnSyncChangedFile(bool isDirectory, string path)
        {
            if (TryGetModelByPath(path, out TModel oldModel))
            {
                if (AskForUserPermission($"Change in file detected, should I update model {oldModel.Name}?"))
                {
                    SyncChangedFile(oldModel, path);
                }
            }
        }

        protected bool TryGetModelByPath(string path, out TModel matchModel)
        {
            matchModel = GetModelByPath(path);
            return matchModel != null;
        }

        protected abstract TModel GetModelByPath(string path);

        protected abstract void SyncRenamedFile(TModel oldModel, string newPath);

        protected abstract void SyncRemovedFile(TModel model);

        protected abstract void SyncCreatedFile(bool isDirectory, string path);

        protected abstract void SyncChangedFile(TModel oldModel, string path);
    }
}
