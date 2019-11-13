using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ForgeModGenerator
{
    public static class BindingExtensions
    {
        private class Dummy : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(Dummy), new PropertyMetadata(null));
        }

        public static T GetResolvedValue<T>(this BindingExpression binding)
        {
            object val = binding.GetResolvedValue();
            return val != null ? (T)val : default;
        }

        public static object GetResolvedValue(this BindingExpression binding) =>
            binding.ResolvedSource.GetType().GetProperty(binding.ResolvedSourcePropertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(binding.ResolvedSource);

        public static T GetResolvedValue<T>(this Binding binding)
        {
            object val = binding.GetResolvedValue();
            return val != null ? (T)val : default;
        }

        public static object GetResolvedValue(this Binding binding)
        {
            Dummy dummy = new Dummy();
            BindingOperations.SetBinding(dummy, Dummy.ValueProperty, binding);
            return dummy.GetValue(Dummy.ValueProperty);
        }
    }
}
