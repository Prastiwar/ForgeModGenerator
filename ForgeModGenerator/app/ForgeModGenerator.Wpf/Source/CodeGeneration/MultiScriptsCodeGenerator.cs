using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class MultiScriptsCodeGenerator : ScriptCodeGenerator/*, IMultiScriptCodeGenerator*/
    {
        public delegate CodeCompileUnit GenerateDelegateHandler();

        public MultiScriptsCodeGenerator(McMod mcMod) : base(mcMod) { }

        public override ClassLocator ScriptLocator => null;

        public abstract Dictionary<ClassLocator, GenerateDelegateHandler> ScriptGenerators { get; }

        protected sealed override CodeCompileUnit CreateTargetCodeUnit() => new CodeCompileUnit();

        public override void RegenerateScript()
        {
            foreach (KeyValuePair<ClassLocator, GenerateDelegateHandler> locator in ScriptGenerators)
            {
                RegenerateScript(locator.Key.FullPath, locator.Value(), GeneratorOptions);
            }
        }
    }
}
