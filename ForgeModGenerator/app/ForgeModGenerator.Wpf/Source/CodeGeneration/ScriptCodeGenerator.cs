using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class ScriptCodeGenerator : IScriptCodeGenerator
    {
        public ScriptCodeGenerator(Mod mod)
        {
            Mod = mod;
            Modname = mod.ModInfo.Name;
            ModnameLower = Modname.ToLower();
            Organization = mod.Organization;
            SourceRootPackageName = $"com.{Organization}.{ModnameLower}";
        }

        public Mod Mod { get; }

        /// <summary> Mod.ModInfo.Name </summary>
        protected string Modname { get; }

        /// <summary> Mod.ModInfo.Name.ToLower() </summary>
        protected string ModnameLower { get; }

        /// <summary> Mod.Organization </summary>
        protected string Organization { get; }

        /// <summary> com.Organization.ModnameLower </summary>
        protected string SourceRootPackageName { get; }

        protected CodeGeneratorOptions GeneratorOptions { get; } = new CodeGeneratorOptions() { BracingStyle = "Block" };

        public abstract ClassLocator ScriptLocator { get; }

        public virtual void RegenerateScript() => RegenerateScript(ScriptLocator.FullPath, CreateTargetCodeUnit(), GeneratorOptions);

        protected void RegenerateScript(string scriptPath, CodeCompileUnit targetCodeUnit, CodeGeneratorOptions options)
        {
            using (JavaCodeProvider javaProvider = new JavaCodeProvider())
            {
                try
                {
                    FileInfo scriptFileInfo = new FileInfo(scriptPath) {
                        IsReadOnly = false
                    };
                    scriptFileInfo.Directory.Create();
                    using (StreamWriter sourceWriter = scriptFileInfo.CreateText())
                    {
                        javaProvider.GenerateCodeFromCompileUnit(targetCodeUnit, sourceWriter, options);
                    }
                    scriptFileInfo.IsReadOnly = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Couldnt generate code for file. Make sure it's not accesed by any process. {scriptPath}", true);
                }
            }
        }

        protected abstract CodeCompileUnit CreateTargetCodeUnit();

        #region Code expression and statements shorthands     
        protected CodeAttributeDeclaration NewAnnotation(string name, params CodeAttributeArgument[] arguments) => new CodeAttributeDeclaration(name, arguments);
        protected CodeAttributeArgument NewAnnotationArg(string valueName, CodeExpression expression) => new CodeAttributeArgument(valueName, expression);
        protected CodeAttributeDeclaration NewOverrideAnnotation() => new CodeAttributeDeclaration("Override");
        protected CodeAttributeDeclaration NewSubscribeEventAnnotation() => new CodeAttributeDeclaration("SubscribeEvent");
        protected CodeAttributeDeclaration NewEventHandlerAnnotation() => new CodeAttributeDeclaration("EventHandler");
        protected CodeAttributeDeclaration NewEventBusSubscriberAnnotation() => new CodeAttributeDeclaration("EventBusSubscriber");
        protected CodeAttributeDeclaration NewInstanceAnnotation() => new CodeAttributeDeclaration("Instance");
        protected CodeAttributeDeclaration NewSideAnnotation(ModSide side) =>
            new CodeAttributeDeclaration("SideOnly", NewAnnotationArg(null, NewFieldReferenceType("Side", side.HasFlag(ModSide.Client) ? "CLIENT" : "SERVER")));

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
            CodeMemberMethod method = new CodeMemberMethod() {
                Name = name,
                ReturnType = NewTypeReference(returnType),
                Attributes = attributes
            };
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
        protected CodeTypeDeclaration NewClassWithMembers(string name, params CodeTypeMember[] members) => NewClassWithMembers(name, TypeAttributes.Public, members);

        protected CodeTypeDeclaration NewClassWithBases(string name, params string[] bases) => NewClassWithBases(name, TypeAttributes.Public, bases);

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
        protected CodeTypeDeclaration NewInterface(string name, params CodeTypeMember[] members) => NewInterface(name, TypeAttributes.Interface | TypeAttributes.Public, members);

        protected CodeTypeDeclaration NewInterface(string name, TypeAttributes attributes, params CodeTypeMember[] members)
        {
            CodeTypeDeclaration newInterface = new CodeTypeDeclaration(name) { IsInterface = true, TypeAttributes = attributes };
            if (members != null)
            {
                newInterface.Members.AddRange(members);
            }
            return newInterface;
        }

        protected CodeNamespace NewPackage(string packageName, CodeTypeDeclaration defaultType, params string[] imports)
        {
            CodeNamespace package = NewPackage(packageName, imports);
            package.Types.Add(defaultType);
            return package;
        }

        protected CodeNamespace NewPackage(string packageName, params string[] imports)
        {
            CodeNamespace package = new CodeNamespace(packageName);
            if (imports != null)
            {
                foreach (string import in imports)
                {
                    package.Imports.Add(NewImport(import));
                }
            }
            return package;
        }

        protected CodeNamespaceImport NewImport(string import) => new CodeNamespaceImport(import);
        protected CodeCompileUnit NewCodeUnit(string packageName, CodeTypeDeclaration defaultType, params string[] imports) => NewCodeUnit(NewPackage(packageName, defaultType, imports));

        protected CodeCompileUnit NewCodeUnit(CodeNamespace package)
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(package);
            return targetUnit;
        }
        #endregion
    }
}
