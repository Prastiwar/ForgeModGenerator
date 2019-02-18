using System.Windows;
using System.Windows.Media;

namespace ForgeModGenerator.Utility
{
    public static class WPFHelper
    {
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);
                    if (frameworkElement != null)
                    {
                        return frameworkElement;
                    }
                }
            }
            return null;
        }

        public static bool ShowOverwriteDialog(string filePath)
        {
            MessageBoxResult result = MessageBox.Show($"File {filePath} already exists.\nDo you want to overwrite it?", "Existing file conflict", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
