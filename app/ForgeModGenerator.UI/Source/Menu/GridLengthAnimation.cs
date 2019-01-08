using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ForgeModGenerator.UI
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType { get { return typeof(GridLength); } }

        protected override System.Windows.Freezable CreateInstanceCore() { return new GridLengthAnimation(); }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength From {
            get { return (GridLength)GetValue(GridLengthAnimation.FromProperty); }
            set { SetValue(GridLengthAnimation.FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength To {
            get { return (GridLength)GetValue(GridLengthAnimation.ToProperty); }
            set { SetValue(GridLengthAnimation.ToProperty, value); }
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
            if (From.GridUnitType != To.GridUnitType) //We can't animate different types, so just skip straight to it
                return To;
            double fromVal = From.Value;
            double toVal = To.Value;
            return new GridLength(
                fromVal > toVal
                    ? (1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal
                    : animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal,
                From.GridUnitType
            );
        }
    }
}
