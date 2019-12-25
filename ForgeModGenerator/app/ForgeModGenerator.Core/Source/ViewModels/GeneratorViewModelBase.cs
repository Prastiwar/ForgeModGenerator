using Prism.Commands;
using System.Collections.Generic;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class GeneratorViewModelBase<TItem> : ViewModelBase
    {
        public GeneratorViewModelBase(GeneratorContext<TItem> context) : base(context.SessionContext)
        {
            Context = context;
            EditorForm = Context.EditorFormFactory.Create();
            EditorForm.ItemEdited += OnItemEdited;
            EditorForm.Validator = Context.Validator;
        }

        public abstract string DirectoryRootPath { get; }

        protected GeneratorContext<TItem> Context { get; }
        protected IEditorForm<TItem> EditorForm { get; }

        private ICommand editItemCommand;
        public ICommand EditItemCommand => editItemCommand ?? (editItemCommand = new DelegateCommand<TItem>(EditItem));

        private ICommand removeItemCommand;
        public ICommand RemoveItemCommand => removeItemCommand ?? (removeItemCommand = new DelegateCommand<TItem>(RemoveItem));

        protected string OnValidate(object sender, IEnumerable<TItem> instances, string propertyName) => Context.Validator.Validate((TItem)sender, instances, propertyName).Error;

        protected abstract void RemoveItem(TItem item);

        protected virtual void EditItem(TItem item) => EditorForm.OpenItemEditor(item);

        protected virtual void OnItemEdited(object sender, ItemEditedEventArgs<TItem> e) { }

        protected abstract void RegenerateCode();
    }
}
