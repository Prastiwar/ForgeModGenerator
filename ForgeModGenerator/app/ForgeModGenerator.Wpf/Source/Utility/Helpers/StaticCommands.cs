using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ForgeModGenerator.Utility
{
    public static class StaticCommands
    {

        private static ICommand showMCItemListCommand;
        public static ICommand ShowMCItemListCommand => showMCItemListCommand ?? (showMCItemListCommand =
            new DelegateCommand<Tuple<ContentControl, string, ItemListForm>>(async (args) => await ShowMCItemList(args.Item1, args.Item2, args.Item3).ConfigureAwait(true)));

        public static async System.Threading.Tasks.Task<bool> ShowMCItemList(ContentControl control, string dialogIdentifier = null, ItemListForm form = null)
        {
            if (form == null)
            {
                form = new ItemListForm();
            }
            bool changed = string.IsNullOrEmpty(dialogIdentifier)
                ? (bool)await DialogHost.Show(form).ConfigureAwait(true)
                : (bool)await DialogHost.Show(form, dialogIdentifier).ConfigureAwait(true);
            if (changed)
            {
                MCItemLocator locator = form.SelectedLocator;
                Image content = (Image)control.Content;
                Uri uriSource = new Uri(locator.ImageFilePath);
                if (!(content.Source is BitmapImage bitmap))
                {
                    bitmap = new BitmapImage(uriSource) {
                        CacheOption = BitmapCacheOption.OnLoad
                    };
                    content.Source = bitmap;
                }
                bitmap.UriSource = uriSource;
            }
            return changed;
        }
    }
}
