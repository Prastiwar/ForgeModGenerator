namespace ForgeModGenerator
{
    public sealed class NoneChoiceFormProvider<TModel> : ModelChoiceFormProvider<TModel>
    {
        public override IUIChoice GetUIChoice() => throw new System.NotSupportedException();
    }
}
