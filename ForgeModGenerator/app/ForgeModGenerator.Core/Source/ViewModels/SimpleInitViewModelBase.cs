using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class SimpleInitViewModelBase<TModel> : GeneratorViewModelBase<TModel>
        where TModel : ObservableDirtyObject, IValidable
    {
        public SimpleInitViewModelBase(GeneratorContext<TModel> context) : base(context) { }

        protected abstract string ScriptFilePath { get; }

        private ICommand openModelEditorCommand;
        public ICommand OpenModelEditorCommand => openModelEditorCommand ?? (openModelEditorCommand = new DelegateCommand<ObservableCollection<TModel>>(CreateNewModel));

        private ICommand openContainerCommand;
        public ICommand OpenContainerCommand => openContainerCommand ?? (openContainerCommand = new DelegateCommand(() => System.Diagnostics.Process.Start(Path.GetDirectoryName(ScriptFilePath))));

        private ObservableCollection<TModel> modelsRepository;
        public ObservableCollection<TModel> ModelsRepository {
            get => modelsRepository;
            set => SetProperty(ref modelsRepository, value);
        }

        public override string DirectoryRootPath => Path.GetDirectoryName(ScriptFilePath);

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                if (ModelsRepository == null)
                {
                    ModelsRepository = new ObservableCollection<TModel>();
                    ModelsRepository.CollectionChanged += OnModelsRepositoryChanged;
                }
                else
                {
                    ModelsRepository.Clear();
                }
                IEnumerable<TModel> foundModels = FindModelsFromScriptFile(ScriptFilePath);
                await AddModelsAsync(foundModels).ConfigureAwait(false);
                Context.Validator?.SetDefaultRepository(ModelsRepository);
                IsLoading = false;
                return true;
            }
            return false;
        }

        /// <summary> Returns content from parantheses block => forString(content) </summary>
        protected string GetParenthesesContentFor(string fromString, string forString, int startLookingForStringIndex = 0)
        {
            int startIndex = fromString.IndexOf(forString, startLookingForStringIndex);
            if (startIndex < 0)
            {
                return "";
            }
            fromString = fromString.Substring(startIndex);
            string content = fromString.GetParenthesesContent();
            return content;
        }

        /// <summary> Returns content from parantheses block => forString(content) </summary>
        protected string[] SplitParenthesesContentFor(string fromString, string forString, int startLookingForStringIndex = 0)
        {
            string content = GetParenthesesContentFor(fromString, forString, startLookingForStringIndex);
            return content == null ? Array.Empty<string>() : content.SplitTrim(',');
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

        protected virtual IEnumerable<TModel> FindModelsFromScriptFile(string scriptFilePath)
        {
            if (!File.Exists(scriptFilePath))
            {
                return Enumerable.Empty<TModel>();
            }
            List<TModel> models = new List<TModel>(8);
            IEnumerable<string> items = File.ReadLines(scriptFilePath).Where(x => x.Trim().StartsWith("public static final"));
            bool firstIsArray = true;
            foreach (string item in items)
            {
                if (!firstIsArray)
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
                else
                {
                    firstIsArray = false;
                    continue;
                }
            }
            return models;
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

        protected override void RemoveItem(TModel item) => ModelsRepository.Remove(item);

        protected string ValidateModel(object sender, string propertyName) => Context.Validator?.Validate((TModel)sender, ModelsRepository, propertyName).Error;

        protected override void OnItemEdited(object sender, ItemEditedEventArgs<TModel> e)
        {
            if (e.Result)
            {
                if (!ModelsRepository.Contains(e.ActualItem))
                {
                    ModelsRepository.Add(e.ActualItem);
                }
                RegenerateCode();
            }
        }

        protected virtual void OnModelsRepositoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.HasFlag(NotifyCollectionChangedAction.Remove))
            {
                RegenerateCode();
            }
        }

        protected override void RegenerateCode()
        {
            McMod mod = SessionContext.SelectedMod;
            Context.CodeGenerationService.RegenerateInitScript(Path.GetFileNameWithoutExtension(ScriptFilePath), mod, ModelsRepository);
        }
    }
}
