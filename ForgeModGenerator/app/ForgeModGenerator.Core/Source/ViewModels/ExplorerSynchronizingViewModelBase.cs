using ForgeModGenerator.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ForgeModGenerator.ViewModels
{
    public abstract class ExplorerSynchronizingViewModelBase<TModel> : SimpleInitViewModelBase<TModel>
        where TModel : ObservableModel
    {
        public ExplorerSynchronizingViewModelBase(GeneratorContext<TModel> context, ISynchronizeInvoke synchronizingObject)
            : base(context) => this.synchronizingObject = synchronizingObject;

        private readonly ISynchronizeInvoke synchronizingObject;

        protected IFolderSynchronizer Synchronizer { get; private set; }

        private string fileSearchPatterns;
        protected string FileSearchPatterns {
            get => fileSearchPatterns;
            set => fileSearchPatterns = string.IsNullOrEmpty(value) ? "*" : value;
        }

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

        protected abstract void OnSyncRenamedFile(bool isDirectory, string oldPath, string newPath);

        protected abstract void OnSyncRemovedFile(bool isDirectory, string path);

        protected abstract void OnSyncCreatedFile(bool isDirectory, string path);

        protected abstract void OnSyncChangedFile(bool isDirectory, string path);
    }
}
