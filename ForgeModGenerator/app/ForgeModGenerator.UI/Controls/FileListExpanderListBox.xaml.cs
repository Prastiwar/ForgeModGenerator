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

        public static readonly DependencyProperty ListTemplateProperty =
            DependencyProperty.Register("ListTemplate", typeof(DataTemplate), typeof(FileListExpanderListBox), new PropertyMetadata(Application.Current.FindResource("DefaultFileListTemplate")));
        public DataTemplate ListTemplate {
            get => (DataTemplate)GetValue(ListTemplateProperty);
            set => SetValue(ListTemplateProperty, value);
        }

        public static readonly DependencyProperty ListItemTemplateProperty =
            DependencyProperty.Register("ListItemTemplate", typeof(DataTemplate), typeof(FileListExpanderListBox), new PropertyMetadata(null));
        public DataTemplate ListItemTemplate {
            get => (DataTemplate)GetValue(ListItemTemplateProperty);
            set => SetValue(ListItemTemplateProperty, value);
        }
    }
}
