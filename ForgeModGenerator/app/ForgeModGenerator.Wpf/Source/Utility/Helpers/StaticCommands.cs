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
        public static ICommand StartProcessCommand => new DelegateCommand<string>((uri) => System.Diagnostics.Process.Start(uri));

        private static ICommand showMCItemListCommand;
        public static ICommand ShowMCItemListCommand => showMCItemListCommand ?? (showMCItemListCommand =
            new DelegateCommand<Tuple<ContentControl, string, ItemListForm>>(async (args) => await ShowMCItemList(args.Item1, args.Item2, args.Item3).ConfigureAwait(true)));

        public static void SetItemButtonImage(ContentControl control, MCItemLocator locator)
        {
            Uri uriSource = new Uri(locator.ImageFilePath);
            if (control.Content == null)
            {
                control.Content = new Image();
            }
            if (control.Content is Image image)
            {
                if (!(image.Source is BitmapImage bitmap))
                {
                    bitmap = new BitmapImage(uriSource) {
                        CacheOption = BitmapCacheOption.OnLoad
                    };
                    image.Source = bitmap;
                }
                bitmap.UriSource = uriSource;
            }
        }

        public static async System.Threading.Tasks.Task<MCItemLocator> ShowMCItemList(string dialogIdentifier = null, ItemListForm form = null)
        {
            if (form == null)
            {
                form = new ItemListForm();
            }
            bool changed = string.IsNullOrEmpty(dialogIdentifier)
                ? (bool)await DialogHost.Show(form).ConfigureAwait(true)
                : (bool)await DialogHost.Show(form, dialogIdentifier).ConfigureAwait(true);
            return changed ? form.SelectedLocator : default;
        }

        public static async System.Threading.Tasks.Task<bool> ShowMCItemList(object control, string dialogIdentifier = null, ItemListForm form = null)
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
                if (control is ContentControl contentControl)
                {
                    SetItemButtonImage(contentControl, form.SelectedLocator);
                }
            }
            return changed;
        }
    }
}
