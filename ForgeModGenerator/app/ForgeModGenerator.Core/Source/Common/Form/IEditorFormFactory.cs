namespace ForgeModGenerator
{
    public interface IEditorFormFactory<TItem>
    {
        IEditorForm<TItem> Create();
    }
}
