using ForgeModGenerator.Models;

namespace ForgeModGenerator.CodeGeneration
{
    public interface IScriptCodeGenerator
    {
        Mod Mod { get; }
        string ScriptFilePath { get; }

        void RegenerateScript();
    }
}
