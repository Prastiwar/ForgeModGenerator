using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class SimpleInitViewModelBase<TModel> : ViewModelBase
        where TModel : ObservableDirtyObject
    {
        public SimpleInitViewModelBase(ISessionContextService sessionContext,
                                        IEditorFormFactory<TModel> editorFormFactory,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext)
        {
            CodeGenerationService = codeGenerationService;
            EditorForm = editorFormFactory.Create();
            EditorForm.ItemEdited += OnModelEdited;
        }

        protected abstract string ScriptFilePath { get; }
        protected ICodeGenerationService CodeGenerationService { get; }
        protected IEditorForm<TModel> EditorForm { get; }

        private ICommand openModelEditor;
        public ICommand OpenModelEditor => openModelEditor ?? (openModelEditor = new DelegateCommand<ObservableCollection<TModel>>(CreateNewModel));

        private ObservableCollection<TModel> modelsRepository;
        public ObservableCollection<TModel> ModelsRepository {
            get => modelsRepository;
            set => SetProperty(ref modelsRepository, value);
        }

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                await AddModelsAsync(FindModelsFromScriptFile(ScriptFilePath)).ConfigureAwait(false);
                RegenerateModels();
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected abstract TModel CreateModelFromLine(string line);
        protected virtual TModel CreateNewEmptyModel() => Activator.CreateInstance<TModel>();

        protected async Task AddModelsAsync(IEnumerable<TModel> models)
        {
            ModelsRepository.Clear();
            foreach (TModel model in models)
            {
                ModelsRepository.Add(model);
                await Task.Delay(1).ConfigureAwait(true);
            }
        }

        protected void CreateNewModel(ObservableCollection<TModel> collection)
        {
            TModel newModel = CreateNewEmptyModel();
            newModel.IsDirty = false;
            collection.Add(newModel);
            EditorForm.OpenItemEditor(newModel);
        }

        protected virtual IEnumerable<TModel> FindModelsFromScriptFile(string scriptFilePath)
        {
            string text = File.ReadAllText(scriptFilePath);
            int startIndex = text.IndexOf("{") + 1;
            int endIndex = text.IndexOf("}", startIndex);
            text = text.Substring(startIndex, endIndex - startIndex);
            IEnumerable<string> items = text.Split(new string[] { "public static final" }, StringSplitOptions.RemoveEmptyEntries).Skip(1);
            List<TModel> models = new List<TModel>(8);
            foreach (string item in items)
            {
                string line = item.Trim();
                TModel model = CreateModelFromLine(line);
                models.Add(model);
            }
            return models;
        }

        protected virtual void OnModelEdited(object sender, ItemEditedEventArgs<TModel> e)
        {
            if (e.Result)
            {
                RegenerateModels();
            }
        }

        protected virtual void RegenerateModels()
        {
            McMod mod = SessionContext.SelectedMod;
            CodeGenerationService.RegenerateInitScript(ScriptFilePath, mod, ModelsRepository);
        }
    }
}
