using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ModHookCodeGenerator : ScriptCodeGenerator
    {
        public ModHookCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.Hook.RelativePath);

        protected override string ScriptFilePath { get; }

        protected CodeMemberField CreateHookString(string variableName, string value) => NewFieldGlobal(typeof(string).FullName, variableName.ToUpper(), NewPrimitive(value));

        protected override CodeCompileUnit CreateTargetCodeUnit() => NewCodeUnit(NewClassWithMembers(SourceCodeLocator.Hook.ClassName, CreateHookString("MODID", Mod.ModInfo.Modid),
                                                                                                                                       CreateHookString("VERSION", Mod.ModInfo.Version),
                                                                                                                                       CreateHookString("ACCEPTEDVERSIONS", Mod.ModInfo.McVersion),
                                                                                                                                       CreateHookString("CLIENTPROXYCLASS", $"{PackageName}.{SourceCodeLocator.ClientProxy.ImportFullName}"),
                                                                                                                                       CreateHookString("SERVERPROXYCLASS", $"{PackageName}.{SourceCodeLocator.ServerProxy.ImportFullName}")));
    }
}
