using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public partial class HorizontalList : UserControl
    {
        public HorizontalList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(HorizontalList), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(HorizontalList), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty StringListProperty =
            DependencyProperty.Register("StringList", typeof(ObservableCollection<string>), typeof(HorizontalList), new PropertyMetadata(null));
        public ObservableCollection<string> StringList {
            get => (ObservableCollection<string>)GetValue(StringListProperty);
            set => SetValue(StringListProperty, value);
        }
    }
}
