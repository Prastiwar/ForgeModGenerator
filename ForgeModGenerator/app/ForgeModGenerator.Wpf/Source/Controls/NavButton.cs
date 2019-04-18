﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavButton), new PropertyMetadata(false));
        public bool IsSelected {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        protected void NavButton_Click()
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
