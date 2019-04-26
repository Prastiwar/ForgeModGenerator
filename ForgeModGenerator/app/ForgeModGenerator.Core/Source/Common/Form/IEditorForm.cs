using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using System;

namespace ForgeModGenerator
{
    public interface IEditorForm<TItem>
    {
        event EventHandler<ItemEditorClosingDialogEventArgs<TItem>> FormClosing;
        event EventHandler<ItemEditedEventArgs<TItem>> ItemEdited;
        event EventHandler<TItem> OpenFormFailed;

        IUIElement Form { get; set; }
        IValidator<TItem> Validator { get; set; }
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

    public class ItemEditorClosingDialogEventArgs<TItem> : DialogClosingEventArgs
    {
        public ItemEditorClosingDialogEventArgs(object result, TItem item) : base(result) => Item = item;
        public TItem Item { get; }
    }

    public class ItemEditorOpeningDialogEventArgs<TItem> : DialogOpenedEventArgs
    {
        public ItemEditorOpeningDialogEventArgs(TItem item) => Item = item;
        public TItem Item { get; }
    }
}
