using ForgeModGenerator.Controls;
using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.SoundGenerator.Controls
{
    public class SoundFolderListBox : FolderListBox
    {
        public SoundFolderListBox() : base() { }

        public static readonly DependencyProperty AddFileAsFolderCommandProperty =
            DependencyProperty.Register("AddFileAsFolderCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand AddFileAsFolderCommand {
            get => (ICommand)GetValue(AddFileAsFolderCommandProperty);
            set => SetValue(AddFileAsFolderCommandProperty, value);
        }
    }
}
