using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    public partial class FileListExpanderListView : UserControl
    {
        public FileListExpanderListView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ListTemplateProperty =
            DependencyProperty.Register("ListTemplate", typeof(DataTemplate), typeof(FileListExpanderListView), new PropertyMetadata(Application.Current.FindResource("DefaultFileListTemplate")));
        public DataTemplate ListTemplate {
            get => (DataTemplate)GetValue(ListTemplateProperty);
            set => SetValue(ListTemplateProperty, value);
        }

        public static readonly DependencyProperty ListItemTemplateProperty =
            DependencyProperty.Register("ListItemTemplate", typeof(DataTemplate), typeof(FileListExpanderListView), new PropertyMetadata(null));
        public DataTemplate ListItemTemplate {
            get => (DataTemplate)GetValue(ListItemTemplateProperty);
            set => SetValue(ListItemTemplateProperty, value);
        }
    }
}
