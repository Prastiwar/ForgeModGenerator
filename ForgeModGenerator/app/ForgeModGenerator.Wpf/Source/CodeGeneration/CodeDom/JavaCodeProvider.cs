using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    [DesignerCategory("Code")]
    public class JavaCodeProvider : CodeDomProvider
    {
        private JavaCodeGenerator generator = new JavaCodeGenerator();

        public override string FileExtension => "java";

        [Obsolete("Use methods directly from CodeDomProvider class")]
        public override ICodeGenerator CreateGenerator() => (ICodeGenerator)generator;

        [Obsolete("JavaCodeProvider does not implement ICodeCompiler interface", true)]
        public override ICodeCompiler CreateCompiler() => null;

        public override TypeConverter GetConverter(Type type)
        {
            if (type == typeof(MemberAttributes))
            {
                return JavaMemberAttributeConverter.Default;
            }
            else if (type == typeof(TypeAttributes))
            {
                return JavaTypeAttributeConverter.Default;
            }
            return base.GetConverter(type);
        }

        public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options) => generator.GenerateCodeFromMember(member, writer, options);

    }
}
