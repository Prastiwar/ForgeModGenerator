using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModelCodeGenerator : ScriptCodeGenerator
    {
        public ModelCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "handler", "IHasModel.java");

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
