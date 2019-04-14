using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(Mod mod) : base(mod) => ScriptLocator = SourceCodeLocator.Hook(mod.ModInfo.Name, mod.Organization);

        public override ClassLocator ScriptLocator { get; }

        protected CodeMemberField CreateHookString(string variableName, string value) => NewFieldGlobal(typeof(string).FullName, variableName.ToUpper(), NewPrimitive(value));

        protected override CodeCompileUnit CreateTargetCodeUnit() => NewCodeUnit(NewClassWithMembers(ScriptLocator.ClassName, 
                                                                CreateHookString("MODID", Mod.ModInfo.Modid),
                                                                CreateHookString("VERSION", Mod.ModInfo.Version),
                                                                CreateHookString("ACCEPTEDVERSIONS", Mod.ModInfo.McVersion),
                                                                CreateHookString("CLIENTPROXYCLASS", $"{PackageName}.{SourceCodeLocator.ClientProxy(Modname, Organization).ImportRelativeName}"),
                                                                CreateHookString("SERVERPROXYCLASS", $"{PackageName}.{SourceCodeLocator.ServerProxy(Modname, Organization).ImportRelativeName}")));
    }
}
