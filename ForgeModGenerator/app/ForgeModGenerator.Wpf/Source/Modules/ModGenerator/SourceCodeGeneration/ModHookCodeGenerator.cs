using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(McMod mcMod) : base(mcMod) => ScriptLocator = SourceCodeLocator.Hook(mcMod.ModInfo.Name, mcMod.Organization);

        public override ClassLocator ScriptLocator { get; }

        protected CodeMemberField CreateHookString(string variableName, string value) => NewFieldGlobal(typeof(string).FullName, variableName.ToUpper(), NewPrimitive(value));

        protected override CodeCompileUnit CreateTargetCodeUnit() => NewCodeUnit(SourceCodeLocator.Hook(Modname, Organization).PackageName,
                                                                NewClassWithMembers(ScriptLocator.ClassName, 
                                                                CreateHookString("NAME", McMod.ModInfo.Name),
                                                                CreateHookString("MODID", McMod.ModInfo.Modid),
                                                                CreateHookString("VERSION", McMod.ModInfo.Version),
                                                                CreateHookString("ACCEPTEDVERSIONS", McMod.ModInfo.McVersion),
                                                                CreateHookString("CLIENTPROXYCLASS", $"{SourceRootPackageName}.{SourceCodeLocator.ClientProxy(Modname, Organization).ImportRelativeName}"),
                                                                CreateHookString("SERVERPROXYCLASS", $"{SourceRootPackageName}.{SourceCodeLocator.ServerProxy(Modname, Organization).ImportRelativeName}")));
    }
}
