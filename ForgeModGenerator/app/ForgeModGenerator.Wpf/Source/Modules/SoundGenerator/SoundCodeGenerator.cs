using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.SoundGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.CodeGeneration
{
    public class SoundCodeGenerator : InitVariablesCodeGenerator<SoundEvent>
    {
        public SoundCodeGenerator(Mod mod) : this(mod, null) { }
        public SoundCodeGenerator(Mod mod, IEnumerable<SoundEvent> soundEvents) : base(mod, soundEvents) => ScriptLocator = SourceCodeLocator.SoundEvents(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

        protected override string GetElementName(SoundEvent element) => element.EventName;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(ScriptLocator.ClassName, "SoundEvent");
            unit.Namespaces[0].Imports.Add(NewImport($"{PackageName}.{SourceCodeLocator.SoundEventBase(Modname, Organization).ImportRelativeName}"));
            return unit;
        }

        protected override IEnumerable<SoundEvent> GetElementsForMod(Mod mod) => base.GetElementsForMod(mod); // TODO: Get SoundEvents for mod

    }
}
