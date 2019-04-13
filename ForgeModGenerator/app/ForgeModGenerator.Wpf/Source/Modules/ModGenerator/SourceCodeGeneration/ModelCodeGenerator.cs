using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModelCodeGenerator : ScriptCodeGenerator
    {
        public ModelCodeGenerator(Mod mod) : base(mod) => 
            ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.ModelInterface.RelativePath);

        public override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit() => 
            NewCodeUnit(NewInterface(SourceCodeLocator.ModelInterface.ClassName, NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public)));
    }
}
