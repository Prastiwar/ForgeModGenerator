using ForgeModGenerator.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FileCollectionListBox : UserControl
    {
        public FileCollectionListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FileItemTemplateProperty =
            DependencyProperty.Register("FileItemTemplate", typeof(DataTemplate), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public DataTemplate FileItemTemplate {
            get => (DataTemplate)GetValue(FileItemTemplateProperty);
            set => SetValue(FileItemTemplateProperty, value);
        }

        public static readonly DependencyProperty FilesSourceProperty =
            DependencyProperty.Register("FilesSource", typeof(ObservableCollection<FileCollection>), typeof(FileCollectionListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ObservableCollection<FileCollection> FilesSource {
            get => (ObservableCollection<FileCollection>)GetValue(FilesSourceProperty);
            set => SetValue(FilesSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedFilesItemProperty =
            DependencyProperty.Register("SelectedFilesItem", typeof(FileCollection), typeof(FileCollectionListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public FileCollection SelectedFilesItem {
            get => (FileCollection)GetValue(SelectedFilesItemProperty);
            set => SetValue(SelectedFilesItemProperty, value);
        }

        public static readonly DependencyProperty EmptyMessageProperty =
            DependencyProperty.Register("EmptyMessage", typeof(string), typeof(FileCollectionListBox), new PropertyMetadata("Select mod"));
        public string EmptyMessage {
            get => (string)GetValue(EmptyMessageProperty);
            set => SetValue(EmptyMessageProperty, value);
        }

        public static readonly DependencyProperty EmptyMessageVisibilityProperty =
            DependencyProperty.Register("EmptyMessageVisibility", typeof(Visibility), typeof(FileCollectionListBox), new PropertyMetadata(Visibility.Visible));
        public Visibility EmptyMessageVisibility {
            get => (Visibility)GetValue(EmptyMessageVisibilityProperty);
            set => SetValue(EmptyMessageVisibilityProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(FileCollectionListBox), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }
    }
}
