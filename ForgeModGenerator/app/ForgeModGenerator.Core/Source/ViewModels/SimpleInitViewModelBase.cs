using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
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
        where TModel : ObservableDirtyObject, IValidable<TModel>
    {
        public SimpleInitViewModelBase(ISessionContextService sessionContext,
                                        IEditorFormFactory<TModel> editorFormFactory,
                                        IUniqueValidator<TModel> validator,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext)
        {
            Validator = validator;
            CodeGenerationService = codeGenerationService;
            EditorForm = editorFormFactory.Create();
            EditorForm.ItemEdited += OnModelEdited;
            EditorForm.Validator = validator;
        }

        protected abstract string ScriptFilePath { get; }
        protected IUniqueValidator<TModel> Validator { get; }
        protected ICodeGenerationService CodeGenerationService { get; }
        protected IEditorForm<TModel> EditorForm { get; }

        private ICommand openModelEditorCommand;
        public ICommand OpenModelEditorCommand => openModelEditorCommand ?? (openModelEditorCommand = new DelegateCommand<ObservableCollection<TModel>>(CreateNewModel));

        private ICommand editModelCommand;
        public ICommand EditModelCommand => editModelCommand ?? (editModelCommand = new DelegateCommand<TModel>(EditModel));

        private ICommand removeModelCommand;
        public ICommand RemoveModelCommand => removeModelCommand ?? (removeModelCommand = new DelegateCommand<TModel>(RemoveModel));

        private ICommand openContainerCommand;
        public ICommand OpenContainerCommand => openContainerCommand ?? (openContainerCommand = new DelegateCommand(() => System.Diagnostics.Process.Start(ScriptFilePath)));

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
                Validator?.SetDefaultRepository(ModelsRepository);
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected abstract TModel ParseModelFromJavaField(string line);
        protected virtual TModel CreateNewEmptyModel()
        {
            TModel model = Activator.CreateInstance<TModel>();
            model.ValidateProperty += ValidateModel;
            return model;
        }

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

        protected virtual void CreateNewModel(ObservableCollection<TModel> collection)
        {
            TModel newModel = CreateNewEmptyModel();
            newModel.IsDirty = false;
            EditorForm.OpenItemEditor(newModel);
        }

        protected virtual void EditModel(TModel model) => EditorForm.OpenItemEditor(model);

        protected virtual void RemoveModel(TModel model) => ModelsRepository.Remove(model);

        protected string ValidateModel(TModel sender, string propertyName) => Validator?.Validate(sender, ModelsRepository, propertyName).Error;

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
