using ForgeModGenerator.Models;
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
        where TModel : ObservableDirtyObject, IValidable, ICopiable
    {
        public SimpleInitViewModelBase(GeneratorContext<TModel> context) : base(context) { }

        protected abstract string InitFilePath { get; }

        private ICommand openModelEditorCommand;
        public ICommand OpenModelEditorCommand => openModelEditorCommand ?? (openModelEditorCommand = new DelegateCommand<ObservableCollection<TModel>>(CreateNewModel));

        private ObservableCollection<TModel> modelsRepository;
        public ObservableCollection<TModel> ModelsRepository {
            get => modelsRepository;
            set => SetProperty(ref modelsRepository, value);
        }

        public override string DirectoryRootPath => Path.GetDirectoryName(InitFilePath);

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
                IEnumerable<TModel> foundModels = FindModels();
                await AddModelsAsync(foundModels).ConfigureAwait(false);
                Context.Validator?.SetDefaultRepository(ModelsRepository);
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected virtual IEnumerable<TModel> FindModels() => FindModelsFromPath(InitFilePath);

        protected virtual IEnumerable<TModel> FindModelsFromPath(string path) => Enumerable.Empty<TModel>();

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
            base.OnItemEdited(sender, e);
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
            Context.CodeGenerationService.GetInitScriptCodeGenerator(Path.GetFileNameWithoutExtension(InitFilePath), mod, ModelsRepository).RegenerateScript();
        }
    }
}
