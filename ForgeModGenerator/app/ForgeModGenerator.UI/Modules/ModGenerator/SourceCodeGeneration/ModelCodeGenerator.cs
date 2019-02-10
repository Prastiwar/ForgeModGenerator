using ForgeModGenerator.CodeGeneration;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModelCodeGenerator : ScriptCodeGenerator
    {
        public ModelCodeGenerator(string modname, string organization) : base(modname, organization)
        {
            string handlerFolder = ModPaths.GeneratedHandlerFolder(modname, organization);
            ScriptFilePath = Path.Combine(handlerFolder, "IHasModel.java");
        }

        protected override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration modelInterface = GetDefaultInterface("IHasModel", false);

            CodeMemberMethod modelRegister = new CodeMemberMethod() {
                Name = "registerModels",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("void")
            };
            modelInterface.Members.Add(modelRegister);

            CodeNamespace package = GetDefaultPackage(modelInterface);
            return GetDefaultCodeUnit(package);
        }
    }
}
