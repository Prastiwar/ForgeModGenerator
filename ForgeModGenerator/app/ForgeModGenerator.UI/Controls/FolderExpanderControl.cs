using ForgeModGenerator.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class FolderExpanderControl : ContentControl
    {
        static FolderExpanderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderExpanderControl), new FrameworkPropertyMetadata(typeof(FolderExpanderControl)));
        }

        public FolderExpanderControl()
        {
            if (FileClickCommand == null)
            {
                FileClickCommand = new RelayCommand<IFileItem>(OpenFile);
            }
            if (ShowContainerCommand == null)
            {
                ShowContainerCommand = new RelayCommand(ShowContainer);
            }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(FolderExpanderControl), new PropertyMetadata("Files"));
        public string HeaderText {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty FolderProperty =
                DependencyProperty.Register("Folder", typeof(IFileFolder), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public IFileFolder Folder {
            get => (IFileFolder)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(object), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public object SelectedFile {
            get => GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
        }

        public static readonly DependencyProperty FileItemTemplateProperty =
            DependencyProperty.Register("FileItemTemplate", typeof(DataTemplate), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public DataTemplate FileItemTemplate {
            get => (DataTemplate)GetValue(FileItemTemplateProperty);
            set => SetValue(FileItemTemplateProperty, value);
        }

        public static readonly DependencyProperty AddFileCommandProperty =
            DependencyProperty.Register("AddFileCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand AddFileCommand {
            get => (ICommand)GetValue(AddFileCommandProperty);
            set => SetValue(AddFileCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveFileCommandProperty =
            DependencyProperty.Register("RemoveFileCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand RemoveFileCommand {
            get => (ICommand)GetValue(RemoveFileCommandProperty);
            set => SetValue(RemoveFileCommandProperty, value);
        }

        public static readonly DependencyProperty FolderFileConverterProperty =
            DependencyProperty.Register("FolderFileConverter", typeof(IMultiValueConverter), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public IMultiValueConverter FolderFileConverter {
            get => (IMultiValueConverter)GetValue(FolderFileConverterProperty);
            set => SetValue(FolderFileConverterProperty, value);
        }

        public static readonly DependencyProperty EditFileCommandProperty =
            DependencyProperty.Register("EditFileCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand EditFileCommand {
            get => (ICommand)GetValue(EditFileCommandProperty);
            set => SetValue(EditFileCommandProperty, value);
        }

        public static readonly DependencyProperty FileClickCommandProperty =
            DependencyProperty.Register("FileClickCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand FileClickCommand {
            get => (ICommand)GetValue(FileClickCommandProperty);
            set => SetValue(FileClickCommandProperty, value);
        }

        public static readonly DependencyProperty ShowContainerCommandProperty =
            DependencyProperty.Register("ShowContainerCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand ShowContainerCommand {
            get => (ICommand)GetValue(ShowContainerCommandProperty);
            set => SetValue(ShowContainerCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveFolderCommandProperty =
            DependencyProperty.Register("RemoveFolderCommand", typeof(ICommand), typeof(FolderExpanderControl), new PropertyMetadata(null));
        public ICommand RemoveFolderCommand {
            get => (ICommand)GetValue(RemoveFolderCommandProperty);
            set => SetValue(RemoveFolderCommandProperty, value);
        }

        protected void OpenFile(IFileItem fileItem) => System.Diagnostics.Process.Start(fileItem.Info.FullName);

        protected void ShowContainer()
        {
            if (Folder != null)
            {
                System.Diagnostics.Process.Start(Folder.Info.FullName);
            }
        }
    }
}
