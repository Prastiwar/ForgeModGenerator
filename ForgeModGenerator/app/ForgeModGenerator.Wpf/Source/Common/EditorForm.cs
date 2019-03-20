using FluentValidation;
using FluentValidation.Results;
using ForgeModGenerator.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.Caching;
using System.Windows;

namespace ForgeModGenerator
{
    public class EditorForm<TItem> : EditorFormBase<TItem> where TItem : ICopiable, IDirty
    {
        public EditorForm(IMemoryCache cache, IDialogService dialogService, FrameworkElement form, AbstractValidator<TItem> validator = null) : base(cache)
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
            Cache.Set(EdiTItemCacheKey, EditingItem.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration); // cache item state so it can be restored later
            bool result = false;
            try
            {
                result = (bool)await DialogHost.Show(Form,
                    (sender, args) => {
                        OnItemEditorOpening(sender, new ItemEditorOpeningDialogEventArgs<TItem>(item));
                    },
                    (sender, args) => {
                        ItemEditorClosingDialogEventArgs<TItem> extArgs = new ItemEditorClosingDialogEventArgs<TItem>(item);
                        OnFormClosing(extArgs);
                        if (extArgs.IsCancelled)
                        {
                            args.Cancel();
                        }
                    });
            }
            catch (Exception)
            {
                OnOpenFormFailed(item);
                return;
            }
            OnItemEdited(new ItemEditedEventArgs<TItem>(result, (TItem)Cache.Get(EdiTItemCacheKey), item));
            Cache.Remove(EdiTItemCacheKey);
        }

        protected override bool CanCloseItemEditor(ItemEditedEventArgs<TItem> e)
        {
            if (e.Result)
            {
                if (Validator != null)
                {
                    ValidationResult validation = Validator.Validate(e.ActualItem);
                    if (!validation.IsValid)
                    {
                        DialogService.ShowMessage("Form is not valid, fix errors before saving", "Invalid form");
                        return false;
                    }
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

        protected virtual void OnItemEditorOpening(object sender, ItemEditorOpeningDialogEventArgs<TItem> args) => Form.DataContext = EditingItem;
    }
}
