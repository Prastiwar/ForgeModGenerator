using ForgeModGenerator.Models;

namespace ForgeModGenerator.CodeGeneration
{
    public interface IScriptCodeGenerator
    {
        McMod McMod { get; }
        ClassLocator ScriptLocator { get; }

        void RegenerateScript();
        void DeleteScript();
    }

    public interface IMultiScriptCodeGenerator : IScriptCodeGenerator
    {
        ClassLocator[] ScriptLocators { get; }
    }
}
