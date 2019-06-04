using Newtonsoft.Json.Serialization;
using System;

namespace ForgeModGenerator.Converters
{
    public class NullToEmptyStringValueProvider : IValueProvider
    {
        private readonly IValueProvider provider;

        public NullToEmptyStringValueProvider(IValueProvider provider) => this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

        public object GetValue(object target) => provider.GetValue(target) ?? "";

        public void SetValue(object target, object value) => provider.SetValue(target, value);
    }
}
