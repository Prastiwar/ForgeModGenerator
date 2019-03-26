using System.Reflection;
using System.Windows.Data;

namespace ForgeModGenerator.Utility
{
    public static class BindingExtensions
    {
        public static T GetResolvedValue<T>(this BindingExpression binding)
        {
            object val = binding.GetResolvedValue();
            return val != null ? (T)val : default;
        }

        public static object GetResolvedValue(this BindingExpression binding) =>
            binding.ResolvedSource.GetType().GetProperty(binding.ResolvedSourcePropertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(binding.ResolvedSource);
    }
}
