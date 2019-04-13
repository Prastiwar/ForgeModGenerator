﻿using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;

namespace ForgeModGenerator.Services
{
    public class CodeGeneratorService : ICodeGenerationService
    {
        public void RegenerateSourceCode(Mod mod)
        {
            foreach (ScriptCodeGenerator generator in ReflectionHelper.EnumerateSubclasses<ScriptCodeGenerator>(mod))
            {
                generator.RegenerateScript();
            }
        }

        public void RegenerateScript<TScriptGenerator>(Mod mod) where TScriptGenerator : IScriptCodeGenerator => ReflectionHelper.CreateInstance<TScriptGenerator>(mod).RegenerateScript();
    }
}
