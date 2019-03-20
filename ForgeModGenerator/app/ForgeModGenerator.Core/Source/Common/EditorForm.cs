using Microsoft.Extensions.Caching.Memory;
using System;

namespace ForgeModGenerator
{
    public abstract class EditorFormBase<TItem> : IEditorForm<TItem> where TItem : ICopiable
    {
        public EditorFormBase(IMemoryCache cache)
        {
            Cache = cache;
            EdiTItemCacheKey = Guid.NewGuid().ToString();
            FormClosing = DefaultOnFormClosing;
        }

        public event EventHandler<TItem> OpenFormFailed;
        public event EventHandler<ItemEditorClosingDialogEventArgs<TItem>> FormClosing;
        public event EventHandler<ItemEditedEventArgs<TItem>> ItemEdited;

        public TItem EditingItem { get; protected set; }

        protected IMemoryCache Cache { get; private set; }

        protected string EdiTItemCacheKey { get; }

        public abstract void OpenItemEditor(TItem item);

        protected virtual bool CanCloseItemEditor(ItemEditedEventArgs<TItem> e) => true;

        protected virtual void OnItemEdited(ItemEditedEventArgs<TItem> e)
        {
            if (ItemEdited == null)
            {
                ItemEdited = DefaultOnItemEdited;
            }
            ItemEdited.Invoke(this, e);
        }

        protected virtual void OnOpenFormFailed(TItem e) => OpenFormFailed?.Invoke(this, e);

        protected virtual void OnFormClosing(ItemEditorClosingDialogEventArgs<TItem> e) => FormClosing.Invoke(this, e);

        private void DefaultOnItemEdited(object sender, ItemEditedEventArgs<TItem> args)
        {
            if (!args.Result)
            {
                args.ActualItem.CopyValues(args.CachedItem);
            }
        }

        private void DefaultOnFormClosing(object sender, ItemEditorClosingDialogEventArgs<TItem> e)
        {
            bool result = e.Result;
            bool canClose = CanCloseItemEditor(new ItemEditedEventArgs<TItem>(result, (TItem)Cache.Get(EdiTItemCacheKey), e.Item));
            if (!canClose)
            {
                e.Cancel();
            }
        }
    }
}
