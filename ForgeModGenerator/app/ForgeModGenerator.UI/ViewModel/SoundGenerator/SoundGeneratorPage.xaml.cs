using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> SoundGenerator UI View-ViewModel </summary>
    public partial class SoundGeneratorPage : Page
    {
        public SoundGeneratorPage()
        {
            InitializeComponent();
        }

        private void BindedClick(object sender, RoutedEventArgs e)
        {
            SoundGeneratorViewModel data = DataContext as SoundGeneratorViewModel;
            if (data != null)
            {
                Button btn = (Button)sender;
                data.SoundClick.Execute(btn.CommandParameter);
            }
        }
    }
}
