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
            if (DataContext is SoundGeneratorViewModel data)
            {
                Button btn = (Button)sender;
                data.SoundClick.Execute(btn.CommandParameter);
            }
        }
    }
}
