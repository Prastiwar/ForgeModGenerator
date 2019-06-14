using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ForgeModGenerator.Converters
{
    public enum Lettercase { Lowercase, Uppercase }

    public class PropertyContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> ignored = new Dictionary<Type, HashSet<string>>();
        private readonly Dictionary<Type, Dictionary<string, string>> renamed = new Dictionary<Type, Dictionary<string, string>>();
        private Lettercase lettercase;
        private bool stringifyNull;

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

        public void SetAllLetterCase(Lettercase lettercase) => this.lettercase = lettercase;

        public void SetNullStringEmpty(bool shouldMakeNullStringEmpty) => stringifyNull = shouldMakeNullStringEmpty;

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
            switch (lettercase)
            {
                case Lettercase.Lowercase:
                    property.PropertyName = property.PropertyName.ToLower();
                    break;
                case Lettercase.Uppercase:
                    property.PropertyName = property.PropertyName.ToUpper();
                    break;
                default:
                    break;
            }
            if (stringifyNull && property.PropertyType == typeof(string))
            {
                property.ValueProvider = new NullToEmptyStringValueProvider(property.ValueProvider);
            }
            return property;
        }

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
