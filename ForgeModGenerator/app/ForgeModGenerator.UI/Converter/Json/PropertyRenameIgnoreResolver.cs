using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace ForgeModGenerator.Converter
{
    public class PropertyRenameIgnoreResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> ignored = new Dictionary<Type, HashSet<string>>();
        private readonly Dictionary<Type, Dictionary<string, string>> renamed = new Dictionary<Type, Dictionary<string, string>>();

        public void IgnoreProperty(Type fromClass, params string[] jsonPropertyNames)
        {
            if (!ignored.ContainsKey(fromClass))
            {
                ignored[fromClass] = new HashSet<string>();
            }

            foreach (string prop in jsonPropertyNames)
            {
                ignored[fromClass].Add(prop);
            }
        }

        public void RenameProperty(Type fromClass, string propertyName, string newJsonPropertyName)
        {
            if (!renamed.ContainsKey(fromClass))
            {
                renamed[fromClass] = new Dictionary<string, string>();
            }
            renamed[fromClass][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = obj => false;
                property.Ignored = true;
            }
            else if (IsRenamed(property.DeclaringType, property.PropertyName, out string newJsonPropertyName))
            {
                property.PropertyName = newJsonPropertyName;
            }
            if (property.DeclaringType == typeof(Model.SoundEvent))
            {
                MessageBox.Show($"{property.DeclaringType} = {property.PropertyName}");
            }
            return property;
        }

        //protected bool IsIgnored(Type fromClass, string jsonPropertyName) => ignored.ContainsKey(fromClass) ? ignored[fromClass].Contains(jsonPropertyName) : false;

        protected bool IsIgnored(Type type, string jsonPropertyName)
        {
            Type ignoredType = null;

            if (ignored.ContainsKey(type))
            {
                ignoredType = type;
            }
            else if (ignored.ContainsKey(type.BaseType))
            {
                ignoredType = type.BaseType;
            }
            return ignoredType != null ? ignored[ignoredType].Contains(jsonPropertyName) : false;
        }

        protected bool IsRenamed(Type fromClass, string jsonPropertyName, out string newJsonPropertyName)
        {
            newJsonPropertyName = null;
            return renamed.ContainsKey(fromClass) ? renamed[fromClass].TryGetValue(jsonPropertyName, out newJsonPropertyName) : false;
        }
    }
}
