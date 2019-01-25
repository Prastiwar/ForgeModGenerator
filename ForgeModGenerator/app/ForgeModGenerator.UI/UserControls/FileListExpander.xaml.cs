using ForgeModGenerator.Model;
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
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(FileListExpander), new PropertyMetadata("Files"));
        public string HeaderText {
            get => $"({FileCollection.Count}) {GetValue(HeaderTextProperty)}";
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty FileCollectionProperty =
            DependencyProperty.Register("FileCollection", typeof(FileCollection), typeof(FileListExpander), new PropertyMetadata(default(FileCollection)));
        public FileCollection FileCollection {
            get => (FileCollection)GetValue(FileCollectionProperty);
            set => SetValue(FileCollectionProperty, value);
        }

        public static readonly DependencyProperty SelectedFilePathProperty =
            DependencyProperty.Register("SelectedFilePath", typeof(string), typeof(FileListExpander), new PropertyMetadata(null));
        public string SelectedFilePath {
            get => (string)GetValue(SelectedFilePathProperty);
            set => SetValue(SelectedFilePathProperty, value);
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

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FileListExpander), new PropertyMetadata(null));
        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private void ShowContainer(object sender, RoutedEventArgs e)
        {
            if (FileCollection != null)
            {
                System.Diagnostics.Process.Start(FileCollection.DestinationPath);
            }
        }
    }
}
