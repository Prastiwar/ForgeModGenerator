using ForgeModGenerator.Models;
using System;
using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class MultiScriptsCodeGenerator : ScriptCodeGenerator
    {
        public MultiScriptsCodeGenerator(Mod mod) : base(mod) { }

        protected abstract string[] ScriptFilePaths { get; }

        public sealed override string ScriptFilePath => throw new InvalidOperationException($"You should not use {nameof(ScriptFilePath)}, use {nameof(ScriptFilePaths)} instead");
        protected sealed override CodeCompileUnit CreateTargetCodeUnit() => new CodeCompileUnit();

        public override void RegenerateScript()
        {
            foreach (string scriptPath in ScriptFilePaths)
            {
                RegenerateScript(scriptPath, CreateTargetCodeUnit(scriptPath), GeneratorOptions);
            }
        }

        protected abstract CodeCompileUnit CreateTargetCodeUnit(string scriptPath);

    }
}
