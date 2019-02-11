using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = ModPaths.GeneratedModHookFile(Modname, Organization);

        protected override string ScriptFilePath { get; }

        protected CodeMemberField CreateHookString(string variableName, string value)
        {
            CodeMemberField field = new CodeMemberField("String", variableName.ToUpper()) {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                InitExpression = new CodePrimitiveExpression(value)
            };
            return field;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration hookClass = GetDefaultClass("Hook", true);

            hookClass.Members.Add(CreateHookString("MODID", Mod.ModInfo.Modid));
            hookClass.Members.Add(CreateHookString("VERSION", Mod.ModInfo.Version));
            hookClass.Members.Add(CreateHookString("ACCEPTEDVERSIONS", Mod.ModInfo.McVersion));
            hookClass.Members.Add(CreateHookString("CLIENTPROXYCLASS", $"{GeneratedPackageName}.proxy.ClientProxy"));
            hookClass.Members.Add(CreateHookString("SERVERPROXYCLASS", $"{GeneratedPackageName}.proxy.ServerProxy"));

            CodeNamespace package = GetDefaultPackage(hookClass);
            return GetDefaultCodeUnit(package);
        }
    }
}
