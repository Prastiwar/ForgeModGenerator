using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.Collections.Generic;

namespace ForgeModGenerator.Services
{
    public interface ICodeGenerationService
    {
        void RegenerateSourceCode(McMod mcMod);
        void RegenerateScript<TScriptGenerator>(McMod mcMod) where TScriptGenerator : IScriptCodeGenerator;
        void RegenerateScript(string className, McMod mcMod);
        void RegenerateInitScript<T>(string className, McMod mcMod, IEnumerable<T> repository);
    }
}
