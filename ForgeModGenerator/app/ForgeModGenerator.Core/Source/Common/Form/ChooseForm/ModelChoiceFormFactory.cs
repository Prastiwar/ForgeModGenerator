using ForgeModGenerator.Services;

namespace ForgeModGenerator
{
    public class ModelChoiceFormFactory<TModel> : IChoiceFormFactory<TModel> where TModel : ICopiable, IDirty
    {
        public ModelChoiceFormFactory(IDialogService dialogService, ModelChoiceFormProvider<TModel> uiProvider)
        {
            this.dialogService = dialogService;
            this.uiProvider = uiProvider;
        }

        private readonly IDialogService dialogService;
        private readonly ModelChoiceFormProvider<TModel> uiProvider;

        public IChoiceForm<TModel> Create() => new ChoiceForm<TModel>(dialogService, uiProvider);
    }
}
