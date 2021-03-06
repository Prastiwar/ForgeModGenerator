﻿using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModelCodeGenerator : ScriptCodeGenerator
    {
        public ModelCodeGenerator(McMod mcMod) : base(mcMod) =>
            ScriptLocator = SourceCodeLocator.ModelInterface(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit() =>
            NewCodeUnit(SourceCodeLocator.ModelInterface(Modname, Organization).PackageName,
                        NewInterface(SourceCodeLocator.ModelInterface(Modname, Organization).ClassName, NewMethod("registerModels", typeof(void).FullName, MemberAttributes.Public)));
    }
}
