using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FolderListBox : UserControl
    {
        public FolderListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FileItemTemplateProperty =
            DependencyProperty.Register("FileItemTemplate", typeof(DataTemplate), typeof(FolderListBox), new PropertyMetadata(null));
        public DataTemplate FileItemTemplate {
            get => (DataTemplate)GetValue(FileItemTemplateProperty);
            set => SetValue(FileItemTemplateProperty, value);
        }

        public static readonly DependencyProperty FoldersSourceProperty =
            DependencyProperty.Register("FoldersSource", typeof(object), typeof(FolderListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object FoldersSource {
            get => GetValue(FoldersSourceProperty);
            set => SetValue(FoldersSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedFolderProperty =
            DependencyProperty.Register("SelectedFolder", typeof(object), typeof(FolderListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedFolder {
            get => GetValue(SelectedFolderProperty);
            set => SetValue(SelectedFolderProperty, value);
        }

        public static readonly DependencyProperty EmptyMessageProperty =
            DependencyProperty.Register("EmptyMessage", typeof(string), typeof(FolderListBox), new PropertyMetadata("Select mod"));
        public string EmptyMessage {
            get => (string)GetValue(EmptyMessageProperty);
            set => SetValue(EmptyMessageProperty, value);
        }

        public static readonly DependencyProperty ShowEmptyMessageProperty =
            DependencyProperty.Register("ShowEmptyMessage", typeof(bool), typeof(FolderListBox), new PropertyMetadata(true));
        public bool ShowEmptyMessage {
            get => (bool)GetValue(ShowEmptyMessageProperty);
            set => SetValue(ShowEmptyMessageProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand EditCommand {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }
    }
}
