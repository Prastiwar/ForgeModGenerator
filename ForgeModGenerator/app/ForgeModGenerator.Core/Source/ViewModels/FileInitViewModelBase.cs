using ForgeModGenerator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Inits TModel from file contents in folder path </summary>
    public abstract class FileInitViewModelBase<TModel> : ExplorerSyncInitViewModelBase<TModel>
        where TModel : ObservableModel
    {
        public FileInitViewModelBase(GeneratorContext<TModel> context, ISynchronizeInvoke synchronizingObject)
            : base(context, synchronizingObject) { }

        protected override IEnumerable<TModel> FindModels() => FindModelsFromPath(DirectoryRootPath);

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
            else
            {
                e.ActualItem.CopyValues(e.CachedItem);
            }
        }

        protected override void RemoveItem(TModel item)
        {
            base.RemoveItem(item);
            Context.CodeGenerationService.GetCustomScriptCodeGenerator(SessionContext.SelectedMod, item).DeleteScript();
        }

        protected override TModel GetModelByPath(string path)
            => ModelsRepository.FirstOrDefault(model => model.Name.ToLower() == Path.GetFileNameWithoutExtension(path).ToLower());

        protected override void SyncRenamedFile(TModel oldModel, string newPath) => SyncChangedFile(oldModel, newPath);

        protected override void SyncRemovedFile(TModel model) => RemoveItem(model);

        protected override void SyncCreatedFile(bool isDirectory, string path)
        {
            if (AskForUserPermission($"New file detected, should I parse it and add new model?"))
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
        }

        protected override void SyncChangedFile(TModel oldModel, string path)
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

        protected abstract TModel ParseModel(string content);
    }
}
