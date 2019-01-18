using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
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
    }
}
