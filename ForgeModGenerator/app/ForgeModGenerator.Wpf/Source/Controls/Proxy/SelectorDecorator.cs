using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ForgeModGenerator.Controls
{
    public static class SelectorDecorator
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable), typeof(SelectorDecorator), new PropertyMetadata(null, ItemsSourcePropertyChanged));

        public static void SetItemsSource(UIElement element, IEnumerable value) => element.SetValue(ItemsSourceProperty, value);
        public static IEnumerable GetItemsSource(UIElement element) => (IEnumerable)element.GetValue(ItemsSourceProperty);

        private static void ItemsSourcePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (!(element is Selector target))
            {
                return;
            }
            int preservedIndex = target.SelectedIndex;
            try
            {
                target.ItemsSource = e.NewValue as IEnumerable;
            }
            finally
            {
                target.SelectedIndex = preservedIndex;
            }
        }
    }
}
