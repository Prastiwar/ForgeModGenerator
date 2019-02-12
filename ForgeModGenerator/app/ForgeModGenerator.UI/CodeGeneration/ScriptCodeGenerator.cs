using ForgeModGenerator.CodeGeneration.JavaCodeDom;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration
{
    public struct Parameter
    {
        public string TypeName { get; }
        public string Name { get; }

        public Parameter(string type, string name)
        {
            TypeName = type;
            Name = name;
        }
    }

    public abstract class ScriptCodeGenerator
    {
        public ScriptCodeGenerator(Mod mod)
        {
            Mod = mod;
            Modname = mod.ModInfo.Name;
            ModnameLower = Modname.ToLower();
            Organization = mod.Organization;
            GeneratedPackageName = $"com.{Organization}.{ModnameLower}.generated";
        }

        protected Mod Mod { get; }
        protected string Modname { get; }
        protected string ModnameLower { get; }
        protected string Organization { get; }
        protected string GeneratedPackageName { get; }
        protected JavaCodeProvider JavaProvider { get; } = new JavaCodeProvider();
        protected CodeGeneratorOptions GeneratorOptions { get; } = new CodeGeneratorOptions() { BracingStyle = "Block" };

        protected abstract string ScriptFilePath { get; }

        public virtual void RegenerateScript() => RegenerateScript(ScriptFilePath, CreateTargetCodeUnit(), GeneratorOptions);

        protected string GetModClassName(string name) => $"{Modname}{name}";

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

        #region Code expression and statements shorthands
        protected CodeParameterDeclarationExpression NewParameter(string type, string name) => new CodeParameterDeclarationExpression(type, name);
        protected CodeVariableReferenceExpression NewVarReference(string variableName) => new CodeVariableReferenceExpression(variableName);
        protected CodeVariableDeclarationStatement NewVariable(string type, string name, CodeExpression initExpression = null) => new CodeVariableDeclarationStatement(type, name, initExpression);

        protected CodeTypeReference NewTypeReference(string typeName) => new CodeTypeReference(typeName);
        protected CodeTypeReferenceExpression NewTypeReferenceExpression(string typeName) => new CodeTypeReferenceExpression(typeName);

        protected CodePrimitiveExpression NewPrimitive(object value) => new CodePrimitiveExpression(value);
        protected CodeThisReferenceExpression NewThis() => new CodeThisReferenceExpression();
        protected CodeSuperConstructorInvokeExpression NewSuper(params CodeExpression[] parameters) => new CodeSuperConstructorInvokeExpression(parameters);

        protected CodeAssignStatement NewAssign(CodeExpression left, CodeExpression right) => new CodeAssignStatement(left, right);
        protected CodeAssignStatement NewAssignVar(string variableName, string secondVariableName) => new CodeAssignStatement(NewVarReference(variableName), NewVarReference(secondVariableName));
        protected CodeAssignStatement NewAssignPrimitive(string variableName, object primitiveValue) => new CodeAssignStatement(NewVarReference(variableName), NewPrimitive(primitiveValue));

        protected CodeFieldReferenceExpression NewFieldReferenceType(string target, string fieldName) => new CodeFieldReferenceExpression(NewTypeReferenceExpression(target), fieldName);
        protected CodeFieldReferenceExpression NewFieldReferenceVar(string target, string fieldName) => new CodeFieldReferenceExpression(NewVarReference(target), fieldName);
        protected CodeMemberField NewField(string type, string name, MemberAttributes attributes, CodeExpression initExpression = null) => new CodeMemberField(type, name) { Attributes = attributes, InitExpression = initExpression };
        protected CodeMemberField NewFieldGlobal(string type, string name, CodeExpression initExpression = null) => NewField(type, name, MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final, initExpression);

        protected CodeMethodReturnStatement NewReturn(CodeExpression expression) => new CodeMethodReturnStatement(expression);
        protected CodeMethodReturnStatement NewReturnThis() => new CodeMethodReturnStatement(NewThis());
        protected CodeMethodReturnStatement NewReturnVar(string variableName) => new CodeMethodReturnStatement(NewVarReference(variableName));
        protected CodeMethodReturnStatement NewReturnPrimitive(object value) => new CodeMethodReturnStatement(NewPrimitive(value));

        protected CodeMethodInvokeExpression NewMethodInvoke(string methodName, params CodeExpression[] arguments) => NewMethodInvoke(null, methodName, arguments);
        protected CodeMethodInvokeExpression NewMethodInvoke(CodeExpression target, string methodName, params CodeExpression[] arguments) => new CodeMethodInvokeExpression(target, methodName, arguments);
        protected CodeMethodInvokeExpression NewMethodInvokeType(string typeName, string methodName, params CodeExpression[] arguments) => NewMethodInvoke(NewTypeReferenceExpression(typeName), methodName, arguments);
        protected CodeMethodInvokeExpression NewMethodInvokeVar(string variableName, string methodName, params CodeExpression[] arguments) => NewMethodInvoke(NewVarReference(variableName), methodName, arguments);

        protected CodeMemberMethod NewMethod(string name, string returnType, MemberAttributes attributes, params Parameter[] parameters)
        {
            CodeMemberMethod method = NewMethod(name, returnType, attributes);
            if (parameters != null)
            {
                foreach (Parameter param in parameters)
                {
                    method.Parameters.Add(NewParameter(param.TypeName, param.Name));
                }
            }
            return method;
        }

        protected CodeConstructor NewConstructor(string name, MemberAttributes attributes, params Parameter[] parameters)
        {
            CodeConstructor ctor = new CodeConstructor() { Name = name, Attributes = attributes };
            if (parameters != null)
            {
                foreach (Parameter param in parameters)
                {
                    ctor.Parameters.Add(NewParameter(param.TypeName, param.Name));
                }
            }
            return ctor;
        }

        protected CodeArrayCreateExpression NewArray(string type, int size) => new CodeArrayCreateExpression(type, size);

        protected CodeObjectCreateExpression NewObject(string type, params CodeExpression[] arguments)
        {
            CodeObjectCreateExpression obj = new CodeObjectCreateExpression(type);
            if (arguments != null)
            {
                obj.Parameters.AddRange(arguments);
            }
            return obj;
        }

        // Gets public class "{Modname}name"
        protected CodeTypeDeclaration NewClassWithMembers(string name, bool useModname = false, params CodeTypeMember[] members) => NewClassWithMembers(useModname ? GetModClassName(name) : name, TypeAttributes.Public, members);

        protected CodeTypeDeclaration NewClassWithBases(string name, bool useModname = false, params string[] bases) => NewClassWithBases(useModname ? GetModClassName(name) : name, TypeAttributes.Public, bases);

        protected CodeTypeDeclaration NewClassWithMembers(string name, TypeAttributes attributes, params CodeTypeMember[] members)
        {
            CodeTypeDeclaration clas = new CodeTypeDeclaration(name) { IsClass = true, TypeAttributes = attributes };
            if (members != null)
            {
                clas.Members.AddRange(members);
            }
            return clas;
        }

        protected CodeTypeDeclaration NewClassWithBases(string name, TypeAttributes attributes, params string[] bases)
        {
            CodeTypeDeclaration clas = new CodeTypeDeclaration(name) { IsClass = true, TypeAttributes = attributes };
            if (bases != null)
            {
                foreach (string baseItem in bases)
                {
                    clas.BaseTypes.Add(baseItem);
                }
            }
            return clas;
        }

        // Gets public interface
        protected CodeTypeDeclaration NewInterface(string name, params CodeTypeMember[] members) => NewInterface(name, TypeAttributes.Public, members);

        protected CodeTypeDeclaration NewInterface(string name, TypeAttributes attributes, params CodeTypeMember[] members)
        {
            CodeTypeDeclaration newInterface = new CodeTypeDeclaration(name) { IsInterface = true, TypeAttributes = attributes };
            if (members != null)
            {
                newInterface.Members.AddRange(members);
            }
            return newInterface;
        }

        protected CodeNamespace NewPackage(CodeTypeDeclaration defaultType, params string[] imports)
        {
            CodeNamespace package = NewPackage(imports);
            package.Types.Add(defaultType);
            return package;
        }

        protected CodeNamespace NewPackage(params string[] imports)
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

        protected CodeCompileUnit NewCodeUnit(CodeTypeDeclaration defaultType, params string[] imports) => NewCodeUnit(NewPackage(imports));

        protected CodeCompileUnit NewCodeUnit(CodeNamespace package)
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(package);
            return targetUnit;
        }
        #endregion
    }
}
