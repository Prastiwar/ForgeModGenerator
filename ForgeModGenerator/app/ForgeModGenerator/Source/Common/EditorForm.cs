using FluentValidation;
using FluentValidation.Results;
using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Runtime.Caching;
using System.Windows;

namespace ForgeModGenerator
{
    public class EditorForm<TItem> where TItem : ICopiable, IDirty
    {
        public class ItemEditedEventArgs : EventArgs
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

        public class ItemEditorClosingDialogEventArgs : EventArgs
        {
            public ItemEditorClosingDialogEventArgs(DialogClosingEventArgs dialogClosingArgs, TItem item)
            {
                Item = item;
                DialogClosingArgs = dialogClosingArgs;
            }
            public DialogClosingEventArgs DialogClosingArgs { get; }
            public TItem Item { get; }
        }

        public class ItemEditorOpeningDialogEventArgs : EventArgs
        {
            public ItemEditorOpeningDialogEventArgs(DialogOpenedEventArgs dialogOpenedEventArgs, TItem item)
            {
                Item = item;
                DialogOpenedArgs = dialogOpenedEventArgs;
            }
            public DialogOpenedEventArgs DialogOpenedArgs { get; }
            public TItem Item { get; }
        }

        public EditorForm(IDialogService dialogService, FrameworkElement form, AbstractValidator<TItem> validator = null)
        {
            DialogService = dialogService;
            Form = form;
            Validator = validator;
        }

        public event EventHandler<ItemEditorClosingDialogEventArgs> FormClosing;
        public event EventHandler<ItemEditedEventArgs> ItemEdited;
        public event EventHandler<TItem> OpenFormFailed;

        public FrameworkElement Form { get; set; }

        public AbstractValidator<TItem> Validator { get; set; }

        public TItem EditingItem { get; protected set; }

        protected IDialogService DialogService { get; set; }

        protected string EdiTItemCacheKey { get; } = Guid.NewGuid().ToString();

        public virtual async void OpenItemEditor(TItem item)
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
                    (sender, args) => { OnItemEditorOpening(sender, new ItemEditorOpeningDialogEventArgs(args, item)); },
                    (sender, args) => { OnItemEditorClosing(sender, new ItemEditorClosingDialogEventArgs(args, item)); });
            }
            catch (Exception)
            {
                OnOpenFormFailed(item);
                return;
            }
            OnItemEdited(new ItemEditedEventArgs(result, (TItem)MemoryCache.Default.Remove(EdiTItemCacheKey), item));
        }

        protected virtual bool CanCloseItemEditor<TEditedArgs>(TEditedArgs args) where TEditedArgs : ItemEditedEventArgs
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

        protected virtual void OnItemEditorOpening(object sender, ItemEditorOpeningDialogEventArgs args) => Form.DataContext = EditingItem;

        protected virtual void OnItemEditorClosing(object sender, ItemEditorClosingDialogEventArgs args)
        {
            bool result = (bool)args.DialogClosingArgs.Parameter;
            bool canClose = CanCloseItemEditor(new ItemEditedEventArgs(result, (TItem)MemoryCache.Default.Get(EdiTItemCacheKey), args.Item));
            FormClosing?.Invoke(this, args);
            if (!canClose)
            {
                args.DialogClosingArgs.Cancel();
            }
        }

        protected void DefaultOnItemEdited(object sender, ItemEditedEventArgs args)
        {
            if (!args.Result)
            {
                args.ActualItem.CopyValues(args.CachedItem);
            }
        }

        protected virtual void OnItemEdited(ItemEditedEventArgs e)
        {
            if (ItemEdited == null)
            {
                ItemEdited = DefaultOnItemEdited;
            }
            ItemEdited.Invoke(this, e);
        }

        protected virtual void OnOpenFormFailed(TItem e) => OpenFormFailed?.Invoke(this, e);
    }
}
