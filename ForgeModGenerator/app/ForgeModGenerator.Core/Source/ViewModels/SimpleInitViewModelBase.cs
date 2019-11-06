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

        private ICommand openContainer;
        public ICommand OpenContainer => openContainer ?? (openContainer = new DelegateCommand(() => System.Diagnostics.Process.Start(ScriptFilePath)));

        private ObservableCollection<TModel> modelsRepository;
        public ObservableCollection<TModel> ModelsRepository {
            get => modelsRepository;
            set => SetProperty(ref modelsRepository, value);
        }

        public string ScriptFileFolderPath => Path.GetDirectoryName(ScriptFilePath);

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                if (ModelsRepository == null)
                {
                    ModelsRepository = new ObservableCollection<TModel>();
                }
                else
                {
                    ModelsRepository.Clear();
                }
                IEnumerable<TModel> foundModels = FindModelsFromScriptFile(ScriptFilePath);
                await AddModelsAsync(foundModels).ConfigureAwait(false);
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected abstract TModel ParseModelFromJavaField(string line);
        protected virtual TModel CreateNewEmptyModel() => Activator.CreateInstance<TModel>();

        protected async Task AddModelsAsync(IEnumerable<TModel> models)
        {
            foreach (TModel model in models)
            {
                ModelsRepository.Add(model);
                await Task.Delay(1).ConfigureAwait(true);
            }
        }

        protected bool TryParseModelFromJavaField(string line, out TModel model)
        {
            try
            {
                model = ParseModelFromJavaField(line);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                model = null;
                return false;
            }
            return model != null;
        }

        protected void CreateNewModel(ObservableCollection<TModel> collection)
        {
            TModel newModel = CreateNewEmptyModel();
            newModel.IsDirty = false;
            EditorForm.OpenItemEditor(newModel);
        }

        protected virtual IEnumerable<TModel> FindModelsFromScriptFile(string scriptFilePath)
        {
            List<TModel> models = new List<TModel>(8);
            IEnumerable<string> items = File.ReadLines(scriptFilePath).Where(x => x.Trim().StartsWith("public static final"));
            foreach (string item in items)
            {
                string line = item.Trim();
                if (TryParseModelFromJavaField(line, out TModel model))
                {
                    models.Add(model);
                }
                else
                {
                    Log.Warning($"Couldn't parse model of type {typeof(TModel)} from java field line {line}");
                }
            }
            return models;
        }

        protected virtual void OnModelEdited(object sender, ItemEditedEventArgs<TModel> e)
        {
            if (e.Result)
            {
                if (!ModelsRepository.Contains(e.ActualItem))
                {
                    ModelsRepository.Add(e.ActualItem);
                }
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
