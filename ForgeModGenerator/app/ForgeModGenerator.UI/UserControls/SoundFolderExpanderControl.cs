using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public class SoundFolderExpanderControl : FolderExpanderControl
    {
        static SoundFolderExpanderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SoundFolderExpanderControl), new FrameworkPropertyMetadata(typeof(SoundFolderExpanderControl)));
        }

        public SoundFolderExpanderControl()
        {

        }

        public static readonly DependencyProperty SoundEventNameChangedCommandProperty =
            DependencyProperty.Register("SoundEventNameChangedCommand", typeof(ICommand), typeof(SoundFolderExpanderControl), new PropertyMetadata(null));
        public ICommand SoundEventNameChangedCommand {
            get { return (ICommand)GetValue(SoundEventNameChangedCommandProperty); }
            set { SetValue(SoundEventNameChangedCommandProperty, value); }
        }
    }
}
