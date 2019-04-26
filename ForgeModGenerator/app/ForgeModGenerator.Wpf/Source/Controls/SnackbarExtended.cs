using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeModGenerator.Controls
{
    public class SnackbarExtended : Snackbar
    {
        public SnackbarExtended() : base()
        {
            IsActiveChanged += OnIsActiveChanged;
            if (!IsActive)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private async void OnIsActiveChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                ((FrameworkElement)sender).Visibility = Visibility.Visible;
            }
            else
            {
                Snackbar s = (Snackbar)sender;
                await Task.Delay(s.DeactivateStoryboardDuration).ConfigureAwait(true);
                ((FrameworkElement)sender).Visibility = Visibility.Collapsed;
            }
        }
    }
}
