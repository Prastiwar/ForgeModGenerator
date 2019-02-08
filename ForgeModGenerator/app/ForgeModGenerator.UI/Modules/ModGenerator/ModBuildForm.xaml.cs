using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.Controls
{
    public partial class ModBuildForm : UserControl
    {
        public ModBuildForm()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ToggleSelectCommandProperty =
            DependencyProperty.Register("ToggleSelectCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand ToggleSelectCommand {
            get => (ICommand)GetValue(ToggleSelectCommandProperty);
            set => SetValue(ToggleSelectCommandProperty, value);
        }

        public static readonly DependencyProperty CompileCommandProperty =
            DependencyProperty.Register("CompileCommand", typeof(ICommand), typeof(ModBuildForm), new PropertyMetadata(null));
        public ICommand CompileCommand {
            get => (ICommand)GetValue(CompileCommandProperty);
            set => SetValue(CompileCommandProperty, value);
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
