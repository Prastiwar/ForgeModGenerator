using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ForgeModGenerator.UserControls
{
    public enum Direction
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public partial class ScrollButton : UserControl
    {
        private readonly DispatcherTimer timer;

        public ScrollButton()
        {
            InitializeComponent();
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(45) };
            if (ScrollCommand == null)
            {
                ScrollCommand = new RelayCommand(() => {
                    timer.Tick += Scroll;
                    timer.Start();
                });
            }
            if (ScrollTarget == null)
            {
                ScrollTarget = new ScrollViewer();
            }
        }

        private Dictionary<Direction, Action> scrollMap;
        protected Dictionary<Direction, Action> ScrollMap {
            get => scrollMap ?? (scrollMap = new Dictionary<Direction, Action>() {
                { Direction.Left, ScrollTarget.LineLeft },
                { Direction.Top, ScrollTarget.LineUp },
                { Direction.Right, ScrollTarget.LineRight },
                { Direction.Bottom, ScrollTarget.LineDown },
            });
        }

        public static readonly DependencyProperty ArrowIconProperty =
            DependencyProperty.Register("ArrowIcon", typeof(string), typeof(ScrollButton), new PropertyMetadata("ArrowUp"));
        public string ArrowIcon {
            get { return (string)GetValue(ArrowIconProperty); }
            set { SetValue(ArrowIconProperty, value); }
        }

        public static readonly DependencyProperty ScrollCommandProperty =
            DependencyProperty.Register("ScrollCommand", typeof(ICommand), typeof(ScrollButton), new PropertyMetadata(null));
        public ICommand ScrollCommand {
            get { return (ICommand)GetValue(ScrollCommandProperty); }
            set { SetValue(ScrollCommandProperty, value); }
        }

        public static readonly DependencyProperty ScrollTargetProperty =
            DependencyProperty.Register("ScrollTarget", typeof(ScrollViewer), typeof(ScrollButton), new PropertyMetadata(null));
        public ScrollViewer ScrollTarget {
            get { return (ScrollViewer)GetValue(ScrollTargetProperty); }
            set { SetValue(ScrollTargetProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(ScrollButton), new PropertyMetadata(Direction.Bottom));
        public Direction Direction {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        private void Scroll(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                timer.Stop();
                timer.Tick -= Scroll;
            }
            ScrollMap[Direction]();
        }
    }
}
