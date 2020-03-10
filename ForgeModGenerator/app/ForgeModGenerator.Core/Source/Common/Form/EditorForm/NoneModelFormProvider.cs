namespace ForgeModGenerator
{
    public sealed class NoneModelFormProvider<TModel> : ModelFormProvider<TModel>
    {
        public override IUIElement GetUIElement() => throw new System.NotSupportedException();
    }
}
