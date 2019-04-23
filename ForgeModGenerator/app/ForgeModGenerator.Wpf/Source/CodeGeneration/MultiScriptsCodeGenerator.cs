using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class MultiScriptsCodeGenerator : ScriptCodeGenerator, IMultiScriptCodeGenerator
    {
        public MultiScriptsCodeGenerator(McMod mcMod) : base(mcMod) { }

        public abstract ClassLocator[] ScriptLocators { get; }

        protected sealed override CodeCompileUnit CreateTargetCodeUnit() => new CodeCompileUnit();

        public override void RegenerateScript()
        {
            foreach (ClassLocator locator in ScriptLocators)
            {
                RegenerateScript(locator.FullPath, CreateTargetCodeUnit(locator.FullPath), GeneratorOptions);
            }
        }

        protected abstract CodeCompileUnit CreateTargetCodeUnit(string scriptPath);

    }
}
