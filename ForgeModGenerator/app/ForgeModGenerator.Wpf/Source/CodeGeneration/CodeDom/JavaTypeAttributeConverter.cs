using System.Reflection;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class JavaTypeAttributeConverter : JavaModifierAttributeConverter
    {
        private JavaTypeAttributeConverter() { }

        private static volatile JavaTypeAttributeConverter defaultConverter;
        public static JavaTypeAttributeConverter Default => defaultConverter ?? (defaultConverter = new JavaTypeAttributeConverter());

        protected override object DefaultValue => TypeAttributes.NotPublic;

        // Attribute names
        private static volatile string[] names;
        protected override string[] Names {
            get {
                if (names == null)
                {
                    names = new string[] {
                        "Public",
                    };
                }
                return names;
            }
        }

        // Attribute values
        private static volatile object[] values;
        protected override object[] Values {
            get {
                if (values == null)
                {
                    values = new object[] {
                        TypeAttributes.Public
                    };
                }
                return values;
            }
        }
    }
}
