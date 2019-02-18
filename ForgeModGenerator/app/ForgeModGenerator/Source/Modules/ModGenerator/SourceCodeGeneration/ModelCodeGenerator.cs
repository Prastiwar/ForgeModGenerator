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

        protected override CodeCompileUnit CreateTargetCodeUnit() => NewCodeUnit(NewInterface("IHasModel", NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public)));
    }
}
