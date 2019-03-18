using System;

namespace ForgeModGenerator
{
    public interface IEditorForm<TItem> where TItem : ICopiable, IDirty
    {
        event EventHandler<ItemEditorClosingDialogEventArgs<TItem>> FormClosing;
        event EventHandler<ItemEditedEventArgs<TItem>> ItemEdited;
        event EventHandler<TItem> OpenFormFailed;

        TItem EditingItem { get; }

        void OpenItemEditor(TItem item);
    }

    public class ItemEditedEventArgs<TItem> : EventArgs
    {
        public ItemEditedEventArgs(bool result, TItem cachedItem, TItem actualItem)
        {
            Result = result;
            CachedItem = cachedItem;
            ActualItem = actualItem;
        }
        public bool Result { get; }
        public TItem CachedItem { get; }
        public TItem ActualItem { get; }
    }

    public class ItemEditorClosingDialogEventArgs<TItem> : EventArgs
    {
        public ItemEditorClosingDialogEventArgs(TItem item) => Item = item;

        public bool Result { get; set; }
        public bool IsCancelled { get; private set; }
        public TItem Item { get; }

        public void Cancel() => IsCancelled = true;
    }

    public class ItemEditorOpeningDialogEventArgs<TItem> : EventArgs
    {
        public ItemEditorOpeningDialogEventArgs(TItem item) => Item = item;
        public TItem Item { get; }
    }
}
