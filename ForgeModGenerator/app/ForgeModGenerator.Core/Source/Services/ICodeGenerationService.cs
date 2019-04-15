using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.Collections.Generic;

namespace ForgeModGenerator.Services
{
    public interface ICodeGenerationService
    {
        void RegenerateSourceCode(Mod mod);
        void RegenerateScript<TScriptGenerator>(Mod mod) where TScriptGenerator : IScriptCodeGenerator;
        void RegenerateScript(string className, Mod mod);
        void RegenerateInitScript<T>(string className, Mod mod, IEnumerable<T> repository);
    }
}
