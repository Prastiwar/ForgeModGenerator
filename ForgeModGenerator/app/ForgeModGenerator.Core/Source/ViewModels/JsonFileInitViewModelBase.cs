using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.ViewModels
{
    public abstract class JsonFileInitViewModelBase<TModel> : FileInitViewModelBase<TModel>
        where TModel : ObservableModel
    {
        public JsonFileInitViewModelBase(GeneratorContext<TModel> context, ISynchronizeInvoke synchronizingObject, IJsonUpdaterFactory<TModel> jsonUpdaterFactory)
            : base(context, synchronizingObject) => JsonUpdater = jsonUpdaterFactory.Create();

        protected IJsonUpdater<TModel> JsonUpdater { get; }

        protected override TModel ParseModel(string content) => JsonUpdater.DeserializeFromContent(content);

        protected override void EditItem(TModel item) => base.EditItem(item);

        protected abstract string GetModelFullPath(TModel model);

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
                    Context.FileSystem.DeleteFile(GetModelFullPath(e.CachedItem), true);
                }
                RegenerateCode(e.ActualItem);
            }
            else
            {
                e.ActualItem.CopyValues(e.CachedItem);
            }
        }
        protected override void RegenerateCode(TModel item)
        {
            McMod mod = SessionContext.SelectedMod;
            DoWithoutSync(() => {
                if (item != null)
                {
                    JsonUpdater.ForceJsonUpdate(item, GetModelFullPath(item));
                }
                Context.CodeGenerationService.GetInitScriptCodeGenerator(Path.GetFileNameWithoutExtension(InitFilePath), mod, ModelsRepository).RegenerateScript();
            });
        }

        protected override void RegenerateCodeBatched(IEnumerable<TModel> models)
        {
            McMod mod = SessionContext.SelectedMod;
            foreach (TModel item in models)
            {
                JsonUpdater.ForceJsonUpdate(item, GetModelFullPath(item));
            }
            Context.CodeGenerationService.GetInitScriptCodeGenerator(Path.GetFileNameWithoutExtension(InitFilePath), mod, ModelsRepository).RegenerateScript();
        }
    }
}
