using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class CodeGenerator<T>
    {
        public CodeGenerator(string modname, string organization)
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

        public virtual void RegenerateScript()
        {
            CodeCompileUnit targetCodeUnit = CreateTargetCodeUnit();
            using (StreamWriter sourceWriter = new StreamWriter(ScriptFilePath))
            {
                JavaProvider.GenerateCodeFromCompileUnit(targetCodeUnit, sourceWriter, GeneratorOptions);
            }
        }

        protected string GetClassName(string name) => $"{Modname}{name}";

        // Gets public class "{Modname}name"
        protected CodeTypeDeclaration GetDefaultClass(string name) => new CodeTypeDeclaration(GetClassName(name)) { IsClass = true, TypeAttributes = TypeAttributes.Public };

        protected CodeNamespace GetDefaultPackage(CodeTypeDeclaration clas, params string[] imports)
        {
            CodeNamespace package = GetDefaultPackage(imports);
            package.Types.Add(clas);
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

        protected abstract CodeCompileUnit CreateTargetCodeUnit();
    }
}
