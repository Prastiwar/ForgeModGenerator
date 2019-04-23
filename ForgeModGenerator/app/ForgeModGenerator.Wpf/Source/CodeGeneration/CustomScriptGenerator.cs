using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class CustomScriptGenerator<T> : ScriptCodeGenerator
    {
        public CustomScriptGenerator(Mod mod, T element) : base(mod) => Element = element;

        protected T Element { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(ScriptLocator.ClassName);
            CodeNamespace package = NewPackage(ScriptLocator.PackageName, clas);
            CodeCompileUnit codeUnit = NewCodeUnit(package);
            codeUnit.UserData[SharedUserData.GenerateWarningMessage] = false;
            return codeUnit;
        }
    }
}
