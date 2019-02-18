using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), Modname + "Hook.java");

        protected override string ScriptFilePath { get; }

        protected CodeMemberField CreateHookString(string variableName, string value) => NewFieldGlobal(typeof(string).FullName, variableName.ToUpper(), NewPrimitive(value));

        protected override CodeCompileUnit CreateTargetCodeUnit() => NewCodeUnit(NewClassWithMembers("Hook", true, CreateHookString("MODID", Mod.ModInfo.Modid),
                                                                                                                   CreateHookString("VERSION", Mod.ModInfo.Version),
                                                                                                                   CreateHookString("ACCEPTEDVERSIONS", Mod.ModInfo.McVersion),
                                                                                                                   CreateHookString("CLIENTPROXYCLASS", $"{GeneratedPackageName}.proxy.ClientProxy"),
                                                                                                                   CreateHookString("SERVERPROXYCLASS", $"{GeneratedPackageName}.proxy.ServerProxy")));
    }
}
