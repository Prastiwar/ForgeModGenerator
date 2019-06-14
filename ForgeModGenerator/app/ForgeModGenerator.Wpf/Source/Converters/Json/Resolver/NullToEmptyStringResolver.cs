using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace ForgeModGenerator.Converters
{
    public class NullToEmptyStringResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);
            if (prop.PropertyType == typeof(string))
            {
                prop.ValueProvider = new NullToEmptyStringValueProvider(prop.ValueProvider);
            }
            return prop;
        }
    }
}
