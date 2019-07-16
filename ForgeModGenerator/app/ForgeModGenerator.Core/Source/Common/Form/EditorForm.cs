using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace ForgeModGenerator
{
    public class EditorForm<TItem> : IEditorForm<TItem> where TItem : ICopiable, IDirty
    {
        public EditorForm(IMemoryCache cache, IDialogService dialogService)
        {
            Cache = cache;
            EdiTItemCacheKey = Guid.NewGuid().ToString();
            FormClosing = DefaultOnFormClosing;
            DialogService = dialogService;
        }

        public event EventHandler<TItem> OpenFormFailed;
        public event EventHandler<ItemEditorClosingDialogEventArgs<TItem>> FormClosing;
        public event EventHandler<ItemEditedEventArgs<TItem>> ItemEdited;

        public IUIElement Form { get; set; }
        public IValidator<TItem> Validator { get; set; }
        public TItem EditingItem { get; protected set; }

        protected IDialogService DialogService { get; set; }
        protected IMemoryCache Cache { get; private set; }
        protected string EdiTItemCacheKey { get; }

        public virtual async void OpenItemEditor(TItem item)
        {
            EditingItem = item;
            if (Form == null)
            {
                return;
            }
            Cache.Set(EdiTItemCacheKey, EditingItem.DeepClone()); // cache item state so it can be restored later
            bool result = false;
            try
            {
                result = (bool)await DialogService.Show(Form,
                    (sender, args) => {
                        OnItemEditorOpening(sender, new ItemEditorOpeningDialogEventArgs<TItem>(item));
                        if (args.ShouldClose)
                        {
                            args.Close();
                        }
                    },
                    (sender, args) => {
                        ItemEditorClosingDialogEventArgs<TItem> extArgs = new ItemEditorClosingDialogEventArgs<TItem>(args.Result, item);
                        OnFormClosing(extArgs);
                        if (extArgs.IsCancelled)
                        {
                            args.Cancel();
                        }
                    }).ConfigureAwait(true);
            }
            catch (Exception)
            {
                OnOpenFormFailed(item);
                return;
            }
            OnItemEdited(new ItemEditedEventArgs<TItem>(result, (TItem)Cache.Get(EdiTItemCacheKey), item));
            Cache.Remove(EdiTItemCacheKey);
        }

        protected virtual bool CanCloseItemEditor(ItemEditedEventArgs<TItem> e)
        {
            if (e.Result)
            {
                bool isValid = true;
                if (Validator != null)
                {
                    isValid = Validator.Validate(e.ActualItem).IsValid;
                }
                else if (e.ActualItem is IValidable validableItem)
                {
                    isValid = validableItem.Validate().IsValid;
                }
                if (!isValid)
                {
                    DialogService.ShowMessage("Form is not valid, fix errors before saving", "Invalid form");
                    return false;
                }
            }
            else
            {
                if (e.ActualItem.IsDirty)
                {
                    return DialogService.ShowMessage("Are you sure you want to exit form? Changes won't be saved", "Unsaved changes", "Yes", "No", null).Result;
                }
            }
            return true;
        }

        protected virtual void OnItemEditorOpening(object sender, ItemEditorOpeningDialogEventArgs<TItem> args) => Form.SetDataContext(EditingItem);

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
            bool result = (bool)e.Result;
            bool canClose = CanCloseItemEditor(new ItemEditedEventArgs<TItem>(result, (TItem)Cache.Get(EdiTItemCacheKey), e.Item));
            if (!canClose)
            {
                e.Cancel();
            }
        }
    }
}
