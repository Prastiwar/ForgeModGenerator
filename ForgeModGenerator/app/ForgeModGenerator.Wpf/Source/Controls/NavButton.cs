using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class NavButton : ContentControl
    {
        static NavButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));

        public static readonly DependencyProperty DescProperty =
            DependencyProperty.Register("Desc", typeof(string), typeof(NavButton), new PropertyMetadata("Descvalue"));
        public string Desc {
            get => (string)GetValue(DescProperty);
            set => SetValue(DescProperty, value);
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(string), typeof(NavButton), new PropertyMetadata("ViewDashboard"));
        public string Kind {
            get => (string)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NavButton), new PropertyMetadata(null));
        public ICommand Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
