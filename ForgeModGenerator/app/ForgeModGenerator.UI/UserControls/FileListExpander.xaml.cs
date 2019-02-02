using ForgeModGenerator.Model;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FileListExpander : UserControl
    {
        public FileListExpander()
        {
            InitializeComponent();
            if (ClickCommand == null)
            {
                ClickCommand = new RelayCommand<IFileItem>(OpenFile);
            }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(FileListExpander), new PropertyMetadata("Files"));
        public string HeaderText {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty FileCollectionProperty =
                DependencyProperty.Register("FileCollection", typeof(IFileFolder), typeof(FileListExpander), new PropertyMetadata(null));
        public IFileFolder FileCollection {
            get => (IFileFolder)GetValue(FileCollectionProperty);
            set => SetValue(FileCollectionProperty, value);
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(object), typeof(FileListExpander), new PropertyMetadata(null));
        public object SelectedFile {
            get => GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FileListExpander), new PropertyMetadata(null));
        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FileListExpander), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(FileListExpander), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(FileListExpander), new PropertyMetadata(null));
        public ICommand EditCommand {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(FileListExpander), new PropertyMetadata(null));

        public ICommand ClickCommand {
            get => (ICommand)GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        private void OpenFile(IFileItem fileItem) => System.Diagnostics.Process.Start(fileItem.Info.FullName);

        private void ShowContainer(object sender, RoutedEventArgs e)
        {
            if (FileCollection != null)
            {
                System.Diagnostics.Process.Start(FileCollection.Info.FullName);
            }
        }
    }
}
