using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class ModBuildForm : UserControl
    {
        public ModBuildForm()
        {
            InitializeComponent();
        }

        protected bool IsSelected { get; set; }

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand SelectCommand {
            get => (ICommand)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        public static readonly DependencyProperty SelectClientCommandProperty =
            DependencyProperty.Register("SelectClientCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand SelectClientCommand {
            get => (ICommand)GetValue(SelectClientCommandProperty);
            set => SetValue(SelectClientCommandProperty, value);
        }

        public static readonly DependencyProperty SelectServerCommandProperty =
            DependencyProperty.Register("SelectServerCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand SelectServerCommand {
            get => (ICommand)GetValue(SelectServerCommandProperty);
            set => SetValue(SelectServerCommandProperty, value);
        }

        public static readonly DependencyProperty RunServerCommandProperty =
            DependencyProperty.Register("RunServerCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand RunServerCommand {
            get => (ICommand)GetValue(RunServerCommandProperty);
            set => SetValue(RunServerCommandProperty, value);
        }

        public static readonly DependencyProperty RunClientCommandProperty =
            DependencyProperty.Register("RunClientCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand RunClientCommand {
            get => (ICommand)GetValue(RunClientCommandProperty);
            set => SetValue(RunClientCommandProperty, value);
        }

        public static readonly DependencyProperty RunBothCommandProperty =
            DependencyProperty.Register("RunBothCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand RunBothCommand {
            get => (ICommand)GetValue(RunBothCommandProperty);
            set => SetValue(RunBothCommandProperty, value);
        }
    }
}
