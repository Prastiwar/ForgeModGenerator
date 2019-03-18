using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class JavaMemberAttributeConverter : JavaModifierAttributeConverter
    {
        private JavaMemberAttributeConverter() { }

        private static volatile JavaMemberAttributeConverter defaultConverter;
        public static JavaMemberAttributeConverter Default => defaultConverter ?? (defaultConverter = new JavaMemberAttributeConverter());

        protected override object DefaultValue => MemberAttributes.Private;

        private static volatile string[] names;
        protected override string[] Names {
            get {
                if (names == null)
                {
                    names = new string[] {
                        "Public",
                        "Protected",
                        "Private"
                    };
                }
                return names;
            }
        }

        private static volatile object[] values;
        protected override object[] Values {
            get {
                if (values == null)
                {
                    values = new object[] {
                        MemberAttributes.Public,
                        MemberAttributes.Family,
                        MemberAttributes.Private
                    };
                }
                return values;
            }
        }
    }
}
