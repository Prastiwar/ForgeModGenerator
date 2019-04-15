using ForgeModGenerator.Models;

namespace ForgeModGenerator.CodeGeneration
{
    public interface IScriptCodeGenerator
    {
        Mod Mod { get; }
        ClassLocator ScriptLocator { get; }

        void RegenerateScript();
    }

    public interface IMultiScriptCodeGenerator : IScriptCodeGenerator
    {
        ClassLocator[] ScriptLocators { get; }
    }
}
