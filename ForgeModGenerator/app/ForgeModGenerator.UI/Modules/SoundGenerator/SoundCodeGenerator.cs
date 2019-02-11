using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using ForgeModGenerator.SoundGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundCodeGenerator : InitVariablesCodeGenerator<SoundEvent>
    {
        public SoundCodeGenerator(Mod mod) : this(mod, null) { }
        public SoundCodeGenerator(Mod mod, IEnumerable<SoundEvent> soundEvents) : base(mod, soundEvents) => ScriptFilePath = ModPaths.GeneratedSoundsFile(Modname, Organization);

        protected override string ScriptFilePath { get; }

        protected override string GetElementName(SoundEvent element) => element.EventName;

        protected override CodeCompileUnit CreateTargetCodeUnit() => CreateDefaultTargetCodeUnit("Sounds", "SoundEvent", "SoundEventBase");

        protected override IEnumerable<SoundEvent> GetElementsForMod(Mod mod) => base.GetElementsForMod(mod); // TODO: Get SoundEvents for mod

    }
}
