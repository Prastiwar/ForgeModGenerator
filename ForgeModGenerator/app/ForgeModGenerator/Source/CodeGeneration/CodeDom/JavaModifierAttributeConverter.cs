using System;
using System.ComponentModel;
using System.Globalization;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public abstract class JavaModifierAttributeConverter : TypeConverter
    {
        protected abstract object[] Values { get; }
        protected abstract string[] Names { get; }
        protected abstract object DefaultValue { get; }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name)
            {
                string[] names = Names;
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i].Equals(name))
                    {
                        return Values[i];
                    }
                }
            }
            return DefaultValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                object[] modifiers = Values;
                for (int i = 0; i < modifiers.Length; i++)
                {
                    if (modifiers[i].Equals(value))
                    {
                        return Names[i];
                    }
                }
                return null;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => new StandardValuesCollection(Values);
    }
}
