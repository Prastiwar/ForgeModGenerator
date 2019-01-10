using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ForgeModGenerator.UI
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType { get { return typeof(GridLength); } }

        protected override Freezable CreateInstanceCore() { return new GridLengthAnimation(); }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength From {
            get { return (GridLength)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength To {
            get { return (GridLength)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public GridLengthAnimation() { }
        public GridLengthAnimation(GridLength from, GridLength to, Duration duration)
        {
            From = from;
            To = to;
            Duration = duration;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            // Animation for different types is not supported
            if (From.GridUnitType != To.GridUnitType)
            {
                return To;
            }
            double fromVal = From.Value;
            double toVal = To.Value;
            return new GridLength(
                fromVal > toVal
                    ? Logic.Math.Lerp(toVal, fromVal, 1 - animationClock.CurrentProgress.Value)
                    : Logic.Math.Lerp(fromVal, toVal, animationClock.CurrentProgress.Value),
                From.GridUnitType
            );
        }
    }
}
