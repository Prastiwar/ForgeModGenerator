using ForgeModGenerator.Model;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.UserControls
{
    public partial class FileListExpanderListBox : UserControl
    {
        public FileListExpanderListBox()
        {
            InitializeComponent();
        }

        // Default is FileListExpander.xaml
        public static readonly DependencyProperty ListTemplateProperty =
            DependencyProperty.Register("ListTemplate", typeof(DataTemplate), typeof(FileListExpanderListBox), new PropertyMetadata(Application.Current.FindResource("DefaultFileListTemplate")));
        public DataTemplate ListTemplate {
            get => (DataTemplate)GetValue(ListTemplateProperty);
            set => SetValue(ListTemplateProperty, value);
        }

        // FileListExpander.xaml connection with its ItemTemplate
        public static readonly DependencyProperty FileListExpanderItemTemplateProperty =
            DependencyProperty.Register("FileListExpanderItemTemplate", typeof(DataTemplate), typeof(FileListExpanderListBox), new PropertyMetadata(null));
        public DataTemplate FileListExpanderItemTemplate {
            get => (DataTemplate)GetValue(FileListExpanderItemTemplateProperty);
            set => SetValue(FileListExpanderItemTemplateProperty, value);
        }

        public static readonly DependencyProperty EmptyMessageProperty =
            DependencyProperty.Register("EmptyMessage", typeof(string), typeof(FileListExpanderListBox), new PropertyMetadata("Select Mod"));
        public string EmptyMessage {
            get => (string)GetValue(EmptyMessageProperty);
            set => SetValue(EmptyMessageProperty, value);
        }
        
        public static readonly DependencyProperty EmptyMessageVisibilityProperty =
            DependencyProperty.Register("EmptyMessageVisibility", typeof(Visibility), typeof(FileListExpanderListBox), new PropertyMetadata(Visibility.Visible));
        public Visibility EmptyMessageVisibility {
            get => (Visibility)GetValue(EmptyMessageVisibilityProperty);
            set => SetValue(EmptyMessageVisibilityProperty, value);
        }
    }
}
