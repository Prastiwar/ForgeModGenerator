using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FolderListBox : UserControl
    {
        public FolderListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(FolderListBox), new PropertyMetadata(null));
        public Style ExpanderStyle {
            get => (Style)GetValue(ExpanderStyleProperty);
            set => SetValue(ExpanderStyleProperty, value);
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

        public static readonly DependencyProperty AddFileCommandProperty =
            DependencyProperty.Register("AddFileCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand AddFileCommand {
            get => (ICommand)GetValue(AddFileCommandProperty);
            set => SetValue(AddFileCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveFileCommandProperty =
            DependencyProperty.Register("RemoveFileCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand RemoveFileCommand {
            get => (ICommand)GetValue(RemoveFileCommandProperty);
            set => SetValue(RemoveFileCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveFolderCommandProperty =
            DependencyProperty.Register("RemoveFolderCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand RemoveFolderCommand {
            get => (ICommand)GetValue(RemoveFolderCommandProperty);
            set => SetValue(RemoveFolderCommandProperty, value);
        }

        public static readonly DependencyProperty AddFolderCommandProperty =
            DependencyProperty.Register("AddFolderCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand AddFolderCommand {
            get => (ICommand)GetValue(AddFolderCommandProperty);
            set => SetValue(AddFolderCommandProperty, value);
        }

        public static readonly DependencyProperty EditFileCommandProperty =
            DependencyProperty.Register("EditFileCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand EditFileCommand {
            get => (ICommand)GetValue(EditFileCommandProperty);
            set => SetValue(EditFileCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveMenuItemConverterProperty =
            DependencyProperty.Register("RemoveMenuItemConverter", typeof(IMultiValueConverter), typeof(FolderListBox), new PropertyMetadata(null));
        public IMultiValueConverter RemoveMenuItemConverter {
            get => (IMultiValueConverter)GetValue(RemoveMenuItemConverterProperty);
            set => SetValue(RemoveMenuItemConverterProperty, value);
        }
    }
}
