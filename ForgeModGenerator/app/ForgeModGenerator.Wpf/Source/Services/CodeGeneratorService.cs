using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.Services
{
    public class CodeGeneratorService : ICodeGenerationService
    {
        public void RegenerateSourceCode(McMod mcMod)
        {
            foreach (ScriptCodeGenerator generator in ReflectionHelper.EnumerateSubclasses<ScriptCodeGenerator>(false, mcMod))
            {
                generator.RegenerateScript();
            }
        }

        public void RegenerateScript<TScriptGenerator>(McMod mcMod) where TScriptGenerator : IScriptCodeGenerator => ReflectionHelper.CreateInstance<TScriptGenerator>(mcMod).RegenerateScript();

        public void RegenerateScript(string className, McMod mcMod)
        {
            foreach (ScriptCodeGenerator generator in ReflectionHelper.EnumerateSubclasses<ScriptCodeGenerator>(false, mcMod))
            {
                if (string.Compare(generator.ScriptLocator.ClassName, className) == 0)
                {
                    generator.RegenerateScript();
                    break;
                }
            }
        }

        public void RegenerateInitScript<T>(string className, McMod mcMod, IEnumerable<T> repository)
        {
            foreach (ScriptCodeGenerator generator in ReflectionHelper.EnumerateSubclasses<InitVariablesCodeGenerator<T>>(false, mcMod, repository))
            {
                if (string.Compare(generator.ScriptLocator.ClassName, className) == 0)
                {
                    generator.RegenerateScript();
                    break;
                }
            }
        }

        public void CreateCustomScript<T>(McMod mcMod, T element)
        {
            CustomScriptGenerator<T> generator = ReflectionHelper.EnumerateSubclasses<CustomScriptGenerator<T>>(false, mcMod, element).FirstOrDefault();
            if (generator != null)
            {
                generator.RegenerateScript();
            }
        }
    }
}
