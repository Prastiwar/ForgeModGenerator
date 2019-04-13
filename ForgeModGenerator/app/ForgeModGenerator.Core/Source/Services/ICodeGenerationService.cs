using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;

namespace ForgeModGenerator.Services
{
    public interface ICodeGenerationService
    {
        void RegenerateSourceCode(Mod mod);
        void RegenerateScript<TScriptGenerator>(Mod mod) where TScriptGenerator : IScriptCodeGenerator;
    }
}
