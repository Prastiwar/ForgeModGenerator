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

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(HorizontalList));
        public ICommand Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        
        public static readonly DependencyProperty CommandArgProperty = DependencyProperty.Register("CommandArg", typeof(ObservableCollection<string>), typeof(HorizontalList));
        public ObservableCollection<string> CommandArg {
            get => (ObservableCollection<string>)GetValue(CommandArgProperty);
            set => SetValue(CommandArgProperty, value);
        }
    }
}
