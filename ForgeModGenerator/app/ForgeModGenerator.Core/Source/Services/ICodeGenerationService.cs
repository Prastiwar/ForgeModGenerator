using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.Collections.Generic;

namespace ForgeModGenerator.Services
{
    public interface ICodeGenerationService
    {
        void RegenerateSourceCode(McMod mcMod);

        IScriptCodeGenerator GetScriptCodeGenerator(string className, McMod mcMod);
        IScriptCodeGenerator GetInitScriptCodeGenerator<T>(string className, McMod mcMod, IEnumerable<T> repository);
        IScriptCodeGenerator GetCustomScriptCodeGenerator<T>(McMod mcMod, T element);
    }
}
