using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.JavaCodeDom
{
    public class JavaMemberAttributeConverter : JavaModifierAttributeConverter
    {
        private static volatile string[] names;
        private static volatile object[] values;
        private static volatile JavaMemberAttributeConverter defaultConverter;

        private JavaMemberAttributeConverter() { }

        public static JavaMemberAttributeConverter Default => defaultConverter ?? (defaultConverter = new JavaMemberAttributeConverter());

        protected override object DefaultValue => MemberAttributes.Private;

        // Attribute names
        protected override string[] Names {
            get {
                if (names == null)
                {
                    names = new string[] {
                        "Public",
                        "Protected",
                        "Protected Internal",
                        "Internal",
                        "Private"
                    };
                }
                return names;
            }
        }

        // Attribute values
        protected override object[] Values {
            get {
                if (values == null)
                {
                    values = new object[] {
                        MemberAttributes.Public,
                        MemberAttributes.Family,
                        MemberAttributes.FamilyOrAssembly,
                        MemberAttributes.Assembly,
                        MemberAttributes.Private
                    };
                }
                return values;
            }
        }
    }
}
