using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Inits TModel from file contents in folder path </summary>
    public abstract class FileInitViewModelBase<TModel> : SimpleInitViewModelBase<TModel>
        where TModel : ObservableModel
    {
        public FileInitViewModelBase(GeneratorContext<TModel> context, ISynchronizeInvoke synchronizingObject)
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

        protected int GetModelIndexByPath(string path)
            => ModelsRepository.FindIndex(model => model.Name.ToLower() == Path.GetFileNameWithoutExtension(path).ToLower());

        protected TModel GetModelByPath(string path)
            => ModelsRepository.FirstOrDefault(model => model.Name.ToLower() == Path.GetFileNameWithoutExtension(path).ToLower());

        protected bool TryGetModelByPath(string path, out TModel matchModel)
        {
            matchModel = GetModelByPath(path);
            return matchModel != null;
        }

        protected virtual void OnSyncRenamedFile(bool isDirectory, string oldPath, string newPath)
        {
            if (TryGetModelByPath(oldPath, out TModel oldModel))
            {
                string fileContent = File.ReadAllText(newPath);
                if (TryParseModel(fileContent, out TModel newModel))
                {
                    ModelsRepository.Remove(oldModel);
                    ModelsRepository.Add(newModel);
                }
                else
                {
                    Log.Warning($"Couldn't parse model of type {typeof(TModel)} from content {fileContent}");
                }
            }
        }

        protected virtual void OnSyncRemovedFile(bool isDirectory, string path)
        {
            if (TryGetModelByPath(path, out TModel model))
            {
                ModelsRepository.Remove(model);
            }
        }

        protected virtual void OnSyncCreatedFile(bool isDirectory, string path)
        {
            string fileContent = File.ReadAllText(path);
            if (TryParseModel(fileContent, out TModel model))
            {
                ModelsRepository.Add(model);
            }
            else
            {
                Log.Warning($"Couldn't parse model of type {typeof(TModel)} from content {fileContent}");
            }
        }

        protected virtual void OnSyncChangedFile(bool isDirectory, string path)
        {
            if (TryGetModelByPath(path, out TModel oldModel))
            {
                string fileContent = File.ReadAllText(path);
                if (TryParseModel(fileContent, out TModel newModel))
                {
                    ModelsRepository.Remove(oldModel);
                    ModelsRepository.Add(newModel);
                }
                else
                {
                    Log.Warning($"Couldn't parse model of type {typeof(TModel)} from content {fileContent}");
                }
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

        protected override IEnumerable<TModel> FindModels() => FindModelsFromPath(DirectoryRootPath);

        protected abstract TModel ParseModel(string content);

        protected bool TryParseModel(string content, out TModel model)
        {
            try
            {
                model = ParseModel(content);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                model = null;
                return false;
            }
            return model != null;
        }

        protected override IEnumerable<TModel> FindModelsFromPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return Enumerable.Empty<TModel>();
            }
            List<TModel> models = new List<TModel>(8);
            foreach (string filePath in Directory.EnumerateFiles(path, FileSearchPatterns, SearchOption.AllDirectories))
            {
                string fileContent = File.ReadAllText(filePath);
                if (TryParseModel(fileContent, out TModel model))
                {
                    models.Add(model);
                }
                else
                {
                    Log.Warning($"Couldn't parse model of type {typeof(TModel)} from content {fileContent}");
                }
            }
            return models;
        }

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<TModel> e)
        {
            if (e.Result)
            {
                if (!ModelsRepository.Contains(e.ActualItem))
                {
                    ModelsRepository.Add(e.ActualItem);
                }
                else
                {
                    Context.CodeGenerationService.GetCustomScriptCodeGenerator(SessionContext.SelectedMod, e.CachedItem).DeleteScript();
                }
                RegenerateCode();
            }
        }

        protected override void RemoveItem(TModel item)
        {
            base.RemoveItem(item);
            Context.CodeGenerationService.GetCustomScriptCodeGenerator(SessionContext.SelectedMod, item).DeleteScript();
        }
    }
}
