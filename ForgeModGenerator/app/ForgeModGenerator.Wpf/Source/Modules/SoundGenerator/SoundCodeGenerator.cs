using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.SoundGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.CodeGeneration
{
    public class SoundCodeGenerator : InitVariablesCodeGenerator<SoundEvent>
    {
        public SoundCodeGenerator(McMod mcMod) : this(mcMod, null) { }
        public SoundCodeGenerator(McMod mcMod, IEnumerable<SoundEvent> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.SoundEvents(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(SoundEvent element) => element.EventName;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "SoundEvent");
            unit.Namespaces[0].Imports.Add(NewImport($"{SourceRootPackageName}.{SourceCodeLocator.SoundEventBase(Modname, Organization).ImportRelativeName}"));
            unit.Namespaces[0].Imports.Add(NewImport($"net.minecraft.util.SoundEvent"));
            return unit;
        }

    }
}
