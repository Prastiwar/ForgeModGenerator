using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ForgeModGenerator.Controls
{
    public class NavButton : ContentControl
    {
        static NavButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(NavButton), new PropertyMetadata("Description", OnDescriptionChanged));
        public string Description {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
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

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavButton), new PropertyMetadata(false));
        public bool IsSelected {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsFirstSelectedProperty =
            DependencyProperty.Register("IsFirstSelected", typeof(bool), typeof(NavButton), new PropertyMetadata(false));
        public bool IsFirstSelected {
            get => (bool)GetValue(IsFirstSelectedProperty);
            set => SetValue(IsFirstSelectedProperty, value);
        }
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavButton navButton = (NavButton)d;
            navButton.ToolTip = navButton.Description;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (IsFirstSelected)
            {
                ClickNavButton();
            }
            base.OnInitialized(e);
        }

        protected void ClickNavButton()
        {
            IsSelected = true;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(Parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(Parent, i);
                if (child is NavButton navBtn)
                {
                    if (navBtn != this)
                    {
                        navBtn.IsSelected = false;
                    }
                }
            }
        }
    }
}
