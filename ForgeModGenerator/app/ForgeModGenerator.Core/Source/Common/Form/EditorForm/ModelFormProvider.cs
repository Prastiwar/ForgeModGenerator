namespace ForgeModGenerator
{
    public abstract class ModelFormProvider<TModel> : IUIElementProvider
    {
        public abstract IUIElement GetUIElement();
    }

    public abstract class ModelChoiceFormProvider<TModel> : IUIChoiceProvider
    {
        public abstract IUIChoice GetUIChoice();
    }
}
