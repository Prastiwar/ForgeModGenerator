using ForgeModGenerator.SoundGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Controls
{
    public partial class SoundEditForm : UserControl
    {
        public SoundEditForm() => InitializeComponent();

        public IEnumerable<Sound.SoundType> SoundTypes => Enum.GetValues(typeof(Sound.SoundType)).Cast<Sound.SoundType>();

        public static readonly DependencyProperty AllSoundsProperty =
            DependencyProperty.Register("AllSounds", typeof(IEnumerable<Sound>), typeof(SoundEditForm), new PropertyMetadata(null));
        public IEnumerable<Sound> AllSounds {
            get => (IEnumerable<Sound>)GetValue(AllSoundsProperty);
            set => SetValue(AllSoundsProperty, value);
        }
    }
}
