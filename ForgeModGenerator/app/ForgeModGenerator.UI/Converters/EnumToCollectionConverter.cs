using System;
using System.ComponentModel;
using System.Reflection;

namespace ForgeModGenerator.Converters
{
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
               : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    string valueString = value.ToString();
                    FieldInfo field = value.GetType().GetField(valueString);
                    if (field != null)
                    {
                        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
                        return attribute != null ? attribute.Description : valueString;
                    }
                }
                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
