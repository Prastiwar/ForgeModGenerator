using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.UI.Controls
{
    public partial class MenuButton : UserControl
    {
        public static readonly DependencyProperty DescProperty =
            DependencyProperty.Register("Desc", typeof(string), typeof(MenuButton), new PropertyMetadata("Descvalue"));
        public string Desc {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(string), typeof(MenuButton), new PropertyMetadata("ViewDashboard"));
        public string Kind {
            get { return (string)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public static RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MenuButton));
        public event RoutedEventHandler Click {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected virtual void OnClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(args);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        public MenuButton()
        {
            InitializeComponent();
        }
    }
}
