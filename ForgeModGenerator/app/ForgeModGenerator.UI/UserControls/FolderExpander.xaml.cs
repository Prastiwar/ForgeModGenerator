using ForgeModGenerator.Model;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public partial class FolderExpander : UserControl
    {
        public FolderExpander()
        {
            InitializeComponent();
            if (ClickCommand == null)
            {
                ClickCommand = new RelayCommand<IFileItem>(OpenFile);
            }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(FolderExpander), new PropertyMetadata("Files"));
        public string HeaderText {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty FolderProperty =
                DependencyProperty.Register("Folder", typeof(IFileFolder), typeof(FolderExpander), new PropertyMetadata(null));
        public IFileFolder Folder {
            get => (IFileFolder)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(object), typeof(FolderExpander), new PropertyMetadata(null));
        public object SelectedFile {
            get => GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FolderExpander), new PropertyMetadata(null));
        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FolderExpander), new PropertyMetadata(null));
        public ICommand AddCommand {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(FolderExpander), new PropertyMetadata(null));
        public ICommand RemoveCommand {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(FolderExpander), new PropertyMetadata(null));
        public ICommand EditCommand {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(FolderExpander), new PropertyMetadata(null));

        public ICommand ClickCommand {
            get => (ICommand)GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        private void OpenFile(IFileItem fileItem) => System.Diagnostics.Process.Start(fileItem.Info.FullName);

        private void ShowContainer(object sender, RoutedEventArgs e)
        {
            if (Folder != null)
            {
                System.Diagnostics.Process.Start(Folder.Info.FullName);
            }
        }
    }
}
