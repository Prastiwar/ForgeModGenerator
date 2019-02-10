using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(McModInfo modInfo, string organization) : base(modInfo.Name, organization)
        {
            this.modInfo = modInfo;
            ScriptFilePath = ModPaths.GeneratedModHookFile(modInfo.Name, organization);
        }

        private readonly McModInfo modInfo;

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

            hookClass.Members.Add(CreateHookString("MODID", modInfo.Modid));
            hookClass.Members.Add(CreateHookString("VERSION", modInfo.Version));
            hookClass.Members.Add(CreateHookString("ACCEPTEDVERSIONS", modInfo.McVersion));
            hookClass.Members.Add(CreateHookString("CLIENTPROXYCLASS", $"{GeneratedPackageName}.proxy.ClientProxy"));
            hookClass.Members.Add(CreateHookString("SERVERPROXYCLASS", $"{GeneratedPackageName}.proxy.ServerProxy"));

            CodeNamespace package = GetDefaultPackage(hookClass);
            return GetDefaultCodeUnit(package);
        }
    }
}
