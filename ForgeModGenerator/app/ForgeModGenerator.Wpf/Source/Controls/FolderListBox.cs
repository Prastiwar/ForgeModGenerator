using ForgeModGenerator.Utility;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class FolderListBox : ContentControl
    {
        static FolderListBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderListBox), new FrameworkPropertyMetadata(typeof(FolderListBox)));

        public FolderListBox()
        {
            if (ShowRootContainerCommand == null)
            {
                ShowRootContainerCommand = new DelegateCommand(ShowContainer);
            }
        }

        public static readonly DependencyProperty FolderTemplateProperty =
            DependencyProperty.Register("FolderTemplate", typeof(DataTemplate), typeof(FolderListBox), new PropertyMetadata(null));
        public DataTemplate FolderTemplate {
            get => (DataTemplate)GetValue(FolderTemplateProperty);
            set => SetValue(FolderTemplateProperty, value);
        }

        public static readonly DependencyProperty RootPathProperty =
            DependencyProperty.Register("RootPath", typeof(string), typeof(FolderListBox), new PropertyMetadata(null));
        public string RootPath {
            get => (string)GetValue(RootPathProperty);
            set => SetValue(RootPathProperty, value);
        }

        public static readonly DependencyProperty FoldersSourceProperty =
            DependencyProperty.Register("FoldersSource", typeof(object), typeof(FolderListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object FoldersSource {
            get => GetValue(FoldersSourceProperty);
            set => SetValue(FoldersSourceProperty, value);
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

        public static readonly DependencyProperty AddFolderCommandProperty =
            DependencyProperty.Register("AddFolderCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand AddFolderCommand {
            get => (ICommand)GetValue(AddFolderCommandProperty);
            set => SetValue(AddFolderCommandProperty, value);
        }

        public static readonly DependencyProperty ShowRootContainerCommandProperty =
            DependencyProperty.Register("ShowRootContainerCommand", typeof(ICommand), typeof(FolderListBox), new PropertyMetadata(null));
        public ICommand ShowRootContainerCommand {
            get => (ICommand)GetValue(ShowRootContainerCommandProperty);
            set => SetValue(ShowRootContainerCommandProperty, value);
        }

        protected void ShowContainer()
        {
            if (IOHelper.IsPathValid(RootPath))
            {
                System.Diagnostics.Process.Start(RootPath);
            }
        }

    }
}
