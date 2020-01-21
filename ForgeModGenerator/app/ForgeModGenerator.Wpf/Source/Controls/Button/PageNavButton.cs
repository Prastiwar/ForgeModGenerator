using ForgeModGenerator.ApplicationModule.ViewModels;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class PageNavButton : NavButton
    {
        static PageNavButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PageNavButton), new FrameworkPropertyMetadata(typeof(PageNavButton)));

        public static readonly DependencyProperty PageKeyProperty =
            DependencyProperty.Register("PageKey", typeof(string), typeof(PageNavButton), new PropertyMetadata(null, OnPageKeyChanged));
        public string PageKey {
            get => (string)GetValue(PageKeyProperty);
            set => SetValue(PageKeyProperty, value);
        }

        private static void OnPageKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageNavButton page = (PageNavButton)d;
            Binding bind = new Binding(".") {
                Source = page.DataContext,
                Converter = Application.Current.FindResource("GetCommandConverter") as IValueConverter,
                ConverterParameter = new object[] { nameof(MainWindowViewModel.GetOpenPageCommand), new Binding("PageKey") { Source = page } }
            };
            page.Command = bind.GetResolvedValue<ICommand>();
        }
    }
}
