using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class ScriptCodeGenerator
    {
        public ScriptCodeGenerator(string modname, string organization)
        {
            Modname = modname;
            Organization = organization;
            PackageName = $"com.{Organization}.{Modname}.generated";
        }

        protected string Modname { get; }
        protected string Organization { get; }
        protected string PackageName { get; }
        protected CodeDomProvider JavaProvider { get; } = new JavaCodeProvider();
        protected CodeGeneratorOptions GeneratorOptions { get; } = new CodeGeneratorOptions() { BracingStyle = "Block" };

        protected abstract string ScriptFilePath { get; }

        public virtual void RegenerateScript() => RegenerateScript(ScriptFilePath, CreateTargetCodeUnit(), GeneratorOptions);

        protected string GetClassName(string name) => $"{Modname}{name}";

        // Gets public class "{Modname}name"
        protected CodeTypeDeclaration GetDefaultClass(string name, bool useModname = true) => new CodeTypeDeclaration(useModname ? GetClassName(name) : name) { IsClass = true, TypeAttributes = TypeAttributes.Public };

        protected CodeNamespace GetDefaultPackage(CodeTypeDeclaration defaultType, params string[] imports)
        {
            CodeNamespace package = GetDefaultPackage(imports);
            package.Types.Add(defaultType);
            return package;
        }

        protected CodeNamespace GetDefaultPackage(params string[] imports)
        {
            CodeNamespace package = new CodeNamespace(PackageName);
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
                using (StreamWriter sourceWriter = new StreamWriter(ScriptFilePath))
                {
                    JavaProvider.GenerateCodeFromCompileUnit(targetCodeUnit, sourceWriter, options);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, $"Couldnt generate code for file. Make sure it's not accesed by any process. {ScriptFilePath}", true);
            }
        }

        protected abstract CodeCompileUnit CreateTargetCodeUnit();
    }
}
