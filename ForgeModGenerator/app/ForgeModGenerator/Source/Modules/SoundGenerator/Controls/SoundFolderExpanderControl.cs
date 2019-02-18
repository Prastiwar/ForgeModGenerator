using ForgeModGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using System.Collections.Generic;
using System.Windows;

namespace ForgeModGenerator.SoundGenerator.Controls
{
    public class SoundFolderExpanderControl : FolderExpanderControl
    {
        static SoundFolderExpanderControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SoundFolderExpanderControl), new FrameworkPropertyMetadata(typeof(SoundFolderExpanderControl)));

        public static readonly DependencyProperty AllFoldersProperty =
            DependencyProperty.Register("AllFolders", typeof(IEnumerable<SoundEvent>), typeof(SoundFolderExpanderControl), new PropertyMetadata(null));
        public IEnumerable<SoundEvent> AllFolders {
            get => (IEnumerable<SoundEvent>)GetValue(AllFoldersProperty);
            set => SetValue(AllFoldersProperty, value);
        }
    }
}
