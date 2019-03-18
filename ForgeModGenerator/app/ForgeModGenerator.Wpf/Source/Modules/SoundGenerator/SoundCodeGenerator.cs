using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.SoundGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;

namespace ForgeModGenerator.SoundGenerator.CodeGeneration
{
    public class SoundCodeGenerator : InitVariablesCodeGenerator<SoundEvent>
    {
        public SoundCodeGenerator(Mod mod) : this(mod, null) { }
        public SoundCodeGenerator(Mod mod, IEnumerable<SoundEvent> soundEvents) : base(mod, soundEvents)
            => ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.SoundEvents.RelativePath);

        protected override string ScriptFilePath { get; }

        protected override string GetElementName(SoundEvent element) => element.EventName;

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = CreateDefaultTargetCodeUnit(SourceCodeLocator.SoundEvents.ClassName, "SoundEvent");
            unit.Namespaces[0].Imports.Add(NewImport($"{PackageName}.{SourceCodeLocator.SoundEventBase.ImportFullName}"));
            return unit;
        }

        protected override IEnumerable<SoundEvent> GetElementsForMod(Mod mod) => base.GetElementsForMod(mod); // TODO: Get SoundEvents for mod

    }
}
