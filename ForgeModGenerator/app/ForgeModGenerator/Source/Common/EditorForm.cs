using FluentValidation;
using FluentValidation.Results;
using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Runtime.Caching;
using System.Windows;

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
        public ItemEditorClosingDialogEventArgs(DialogClosingEventArgs dialogClosingArgs, TItem item)
        {
            Item = item;
            DialogClosingArgs = dialogClosingArgs;
        }
        public DialogClosingEventArgs DialogClosingArgs { get; }
        public TItem Item { get; }
    }

    public class ItemEditorOpeningDialogEventArgs<TItem> : EventArgs
    {
        public ItemEditorOpeningDialogEventArgs(DialogOpenedEventArgs dialogOpenedEventArgs, TItem item)
        {
            Item = item;
            DialogOpenedArgs = dialogOpenedEventArgs;
        }
        public DialogOpenedEventArgs DialogOpenedArgs { get; }
        public TItem Item { get; }
    }

    public abstract class EditorFormBase<TItem> : IEditorForm<TItem> where TItem : ICopiable, IDirty
    {
        public event EventHandler<ItemEditorClosingDialogEventArgs<TItem>> FormClosing;
        public event EventHandler<ItemEditedEventArgs<TItem>> ItemEdited;
        public event EventHandler<TItem> OpenFormFailed;

        public TItem EditingItem { get; protected set; }

        protected string EdiTItemCacheKey { get; } = Guid.NewGuid().ToString();

        public abstract void OpenItemEditor(TItem item);

        protected virtual void OnItemEdited(ItemEditedEventArgs<TItem> e)
        {
            if (ItemEdited == null)
            {
                ItemEdited = DefaultOnItemEdited;
            }
            ItemEdited.Invoke(this, e);
        }

        protected virtual void OnOpenFormFailed(TItem e) => OpenFormFailed?.Invoke(this, e);

        protected virtual void OnFormClosing(ItemEditorClosingDialogEventArgs<TItem>  args) => FormClosing?.Invoke(this, args);

        protected void DefaultOnItemEdited(object sender, ItemEditedEventArgs<TItem> args)
        {
            if (!args.Result)
            {
                args.ActualItem.CopyValues(args.CachedItem);
            }
        }
    }

    public class EditorForm<TItem> : EditorFormBase<TItem> where TItem : ICopiable, IDirty
    {
        public EditorForm(IDialogService dialogService, FrameworkElement form, AbstractValidator<TItem> validator = null)
        {
            DialogService = dialogService;
            Form = form;
            Validator = validator;
        }

        public FrameworkElement Form { get; set; }

        public AbstractValidator<TItem> Validator { get; set; }

        protected IDialogService DialogService { get; set; }

        public override async void OpenItemEditor(TItem item)
        {
            EditingItem = item;
            if (Form == null)
            {
                return;
            }
            MemoryCache.Default.Set(EdiTItemCacheKey, EditingItem.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration); // cache item state so it can be restored later
            bool result = false;
            try
            {
                result = (bool)await DialogHost.Show(Form,
                    (sender, args) => { OnItemEditorOpening(sender, new ItemEditorOpeningDialogEventArgs<TItem>(args, item)); },
                    (sender, args) => { OnItemEditorClosing(sender, new ItemEditorClosingDialogEventArgs<TItem>(args, item)); });
            }
            catch (Exception)
            {
                OnOpenFormFailed(item);
                return;
            }
            OnItemEdited(new ItemEditedEventArgs<TItem>(result, (TItem)MemoryCache.Default.Remove(EdiTItemCacheKey), item));
        }

        protected virtual bool CanCloseItemEditor<TEditedArgs>(TEditedArgs args) where TEditedArgs : ItemEditedEventArgs<TItem>
        {
            if (args.Result)
            {
                if (Validator != null)
                {
                    ValidationResult validation = Validator.Validate(args.ActualItem);
                    if (!validation.IsValid)
                    {
                        DialogService.ShowMessage("Form is not valid, fix errors before saving", "Invalid form");
                        return false;
                    }
                }
            }
            else
            {
                if (args.ActualItem.IsDirty)
                {
                    return DialogService.ShowMessage("Are you sure you want to exit form? Changes won't be saved", "Unsaved changes", "Yes", "No", null).Result;
                }
            }
            return true;
        }

        protected virtual void OnItemEditorOpening(object sender, ItemEditorOpeningDialogEventArgs<TItem> args) => Form.DataContext = EditingItem;

        protected virtual void OnItemEditorClosing(object sender, ItemEditorClosingDialogEventArgs<TItem> args)
        {
            bool result = (bool)args.DialogClosingArgs.Parameter;
            bool canClose = CanCloseItemEditor(new ItemEditedEventArgs<TItem>(result, (TItem)MemoryCache.Default.Get(EdiTItemCacheKey), args.Item));
            OnFormClosing(args);
            if (!canClose)
            {
                args.DialogClosingArgs.Cancel();
            }
        }
    }
}
