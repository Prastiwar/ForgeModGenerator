using System.Collections.Generic;

namespace ForgeModGenerator.ViewModels
{
    public abstract class GeneratorViewModelBase<TItem> : ViewModelBase
    {
        public GeneratorViewModelBase(GeneratorContext<TItem> context)
            : base(context.SessionContext)
        {
            Context = context;
            EditorForm = Context.EditorFormFactory.Create();
            EditorForm.ItemEdited += OnItemEdited;
            EditorForm.Validator = Context.Validator;
        }

        protected GeneratorContext<TItem> Context { get; }

        protected IEditorForm<TItem> EditorForm { get; }

        protected string OnValidate(object sender, IEnumerable<TItem> instances, string propertyName) => Context.Validator.Validate((TItem)sender, instances, propertyName).Error;

        protected virtual void OnItemEdited(object sender, ItemEditedEventArgs<TItem> e) { }

        protected abstract void RegenerateCode();
    }
}
