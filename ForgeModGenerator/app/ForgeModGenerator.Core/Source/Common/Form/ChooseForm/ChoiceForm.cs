using ForgeModGenerator.Services;
using System;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public class ChoiceForm<TItem> : IChoiceForm<TItem> where TItem : ICopiable, IDirty
    {
        public ChoiceForm(IDialogService dialogService, IUIChoiceProvider formProvider)
        {
            DialogService = dialogService;
            FormProvider = formProvider;
        }

        protected IDialogService DialogService { get; set; }
        protected IUIChoiceProvider FormProvider { get; set; }
        protected IUIChoice UsedForm { get; set; }

        public object OpenChoices() => OpenChoicesAsync().Result;

        public virtual async Task<object> OpenChoicesAsync()
        {
            UsedForm = FormProvider?.GetUIChoice();
            bool result = false;
            try
            {
                result = (bool)await DialogService.Show(UsedForm).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't open form");
                return null;
            }
            if (result)
            {
                return UsedForm.SelectedValue;
            }
            return null;
        }
    }
}
