namespace ForgeModGenerator
{
    public abstract class ModelFormProvider<TModel> : IUIElementProvider
    {
        public abstract IUIElement GetUIElement();
    }
}
