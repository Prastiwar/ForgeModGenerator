using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class ScriptCodeGenerator
    {
        public ScriptCodeGenerator(Mod mod)
        {
            Mod = mod;
            Modname = mod.ModInfo.Name;
            Organization = mod.Organization;
            GeneratedPackageName = $"com.{Organization}.{Modname}.generated";
        }

        protected Mod Mod { get; }
        protected string Modname { get; }
        protected string Organization { get; }
        protected string GeneratedPackageName { get; }
        protected JavaCodeProvider JavaProvider { get; } = new JavaCodeProvider();
        protected CodeGeneratorOptions GeneratorOptions { get; } = new CodeGeneratorOptions() { BracingStyle = "Block" };

        protected abstract string ScriptFilePath { get; }

        public virtual void RegenerateScript() => RegenerateScript(ScriptFilePath, CreateTargetCodeUnit(), GeneratorOptions);

        protected string GetClassName(string name) => $"{Modname}{name}";

        // Gets public class "{Modname}name"
        protected CodeTypeDeclaration GetDefaultClass(string name, bool useModname = false) => new CodeTypeDeclaration(useModname ? GetClassName(name) : name) { IsClass = true, TypeAttributes = TypeAttributes.Public };
        protected CodeTypeDeclaration GetDefaultInterface(string name, bool useModname = false) => new CodeTypeDeclaration(useModname ? GetClassName(name) : name) { IsInterface = true, TypeAttributes = TypeAttributes.Public };

        protected CodeNamespace GetDefaultPackage(CodeTypeDeclaration defaultType, params string[] imports)
        {
            CodeNamespace package = GetDefaultPackage(imports);
            package.Types.Add(defaultType);
            return package;
        }

        protected CodeNamespace GetDefaultPackage(params string[] imports)
        {
            CodeNamespace package = new CodeNamespace(GeneratedPackageName);
            if (imports != null)
            {
                foreach (string import in imports)
                {
                    package.Imports.Add(new CodeNamespaceImport(import));
                }
            }
            return package;
        }

        protected CodeCompileUnit GetDefaultCodeUnit(CodeNamespace package)
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(package);
            return targetUnit;
        }

        protected void RegenerateScript(string scriptPath, CodeCompileUnit targetCodeUnit, CodeGeneratorOptions options)
        {
            try
            {
                new FileInfo(scriptPath).Directory.Create();
                using (StreamWriter sourceWriter = new StreamWriter(scriptPath))
                {
                    JavaProvider.GenerateCodeFromCompileUnit(targetCodeUnit, sourceWriter, options);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, $"Couldnt generate code for file. Make sure it's not accesed by any process. {scriptPath}", true);
            }
        }

        protected abstract CodeCompileUnit CreateTargetCodeUnit();
    }
}
