using System.Windows;

namespace ForgeModGenerator.UserControls
{
    public class SoundFolderExpanderControl : FolderExpanderControl
    {
        static SoundFolderExpanderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SoundFolderExpanderControl), new FrameworkPropertyMetadata(typeof(SoundFolderExpanderControl)));
        }
    }
}
