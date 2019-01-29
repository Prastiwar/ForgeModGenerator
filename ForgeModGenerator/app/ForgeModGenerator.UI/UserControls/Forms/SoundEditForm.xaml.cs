using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class SoundEditForm : UserControl
    {
        public SoundEditForm()
        {
            InitializeComponent();
        }

        public IEnumerable<Sound.SoundType> SoundTypes => Enum.GetValues(typeof(Sound.SoundType)).Cast<Sound.SoundType>();

        public static readonly DependencyProperty AddSoundCommandProperty =
            DependencyProperty.Register("AddSoundCommand", typeof(ICommand), typeof(SoundEditForm), new PropertyMetadata(null));
        public ICommand AddSoundCommand {
            get => (ICommand)GetValue(AddSoundCommandProperty);
            set => SetValue(AddSoundCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteSoundCommandProperty =
            DependencyProperty.Register("DeleteSoundCommand", typeof(ICommand), typeof(SoundEditForm), new PropertyMetadata(null));
        public ICommand DeleteSoundCommand {
            get => (ICommand)GetValue(DeleteSoundCommandProperty);
            set => SetValue(DeleteSoundCommandProperty, value);
        }

        public static readonly DependencyProperty ChangeSoundCommandProperty =
            DependencyProperty.Register("ChangeSoundCommand", typeof(ICommand), typeof(SoundEditForm), new PropertyMetadata(null));
        public ICommand ChangeSoundCommand {
            get => (ICommand)GetValue(ChangeSoundCommandProperty);
            set => SetValue(ChangeSoundCommandProperty, value);
        }

    }
}
