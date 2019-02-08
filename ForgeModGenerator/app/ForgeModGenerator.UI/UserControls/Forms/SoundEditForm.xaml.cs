using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.UserControls
{
    public partial class SoundEditForm : UserControl
    {
        public SoundEditForm()
        {
            InitializeComponent();
            Loaded += SoundEditForm_Loaded;
        }

        public bool IsValid() => !Validation.GetHasError(NameBox);

        private void SoundEditForm_Loaded(object sender, RoutedEventArgs e)
        {
            NameBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        public IEnumerable<Sound.SoundType> SoundTypes => Enum.GetValues(typeof(Sound.SoundType)).Cast<Sound.SoundType>();

        public static readonly DependencyProperty AllSoundsProperty =
            DependencyProperty.Register("AllSounds", typeof(IEnumerable<Sound>), typeof(SoundEditForm), new PropertyMetadata(null));
        public IEnumerable<Sound> AllSounds {
            get { return (IEnumerable<Sound>)GetValue(AllSoundsProperty); }
            set { SetValue(AllSoundsProperty, value); }
        }
    }
}
