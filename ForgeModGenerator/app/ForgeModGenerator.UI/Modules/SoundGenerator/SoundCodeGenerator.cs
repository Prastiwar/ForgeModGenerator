using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.SoundGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundCodeGenerator : ScriptCodeGenerator
    {
        public SoundCodeGenerator(string modname, string organization, IEnumerable<SoundEvent> soundEvents) : base(modname, organization)
        {
            SoundEvents = soundEvents;
            ScriptFilePath = ModPaths.GeneratedSoundsFile(modname, organization);
        }

        protected IEnumerable<SoundEvent> SoundEvents { get; }

        protected override string ScriptFilePath { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            CodeTypeDeclaration soundsClass = GetDefaultClass("Sounds");

            CodeMemberField listField = new CodeMemberField("List<SoundEvent>", "SOUNDS") {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                InitExpression = new CodeObjectCreateExpression("ArrayList<SoundEvent>")
            };

            soundsClass.Members.Add(listField);
            foreach (SoundEvent soundEvent in SoundEvents)
            {
                CodeMemberField field = new CodeMemberField("SoundEvent", soundEvent.EventName.Replace(' ', '_').ToUpper()) {
                    Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                    InitExpression = new CodeObjectCreateExpression("SoundEventBase", new CodePrimitiveExpression(soundEvent.EventName))
                };
                soundsClass.Members.Add(field);
            }

            CodeNamespace package = GetDefaultPackage(soundsClass, "java.util.ArrayList", "java.util.List", "net.minecraft.util.SoundEvent");
            targetUnit.Namespaces.Add(package);

            return targetUnit;
        }
    }
}
