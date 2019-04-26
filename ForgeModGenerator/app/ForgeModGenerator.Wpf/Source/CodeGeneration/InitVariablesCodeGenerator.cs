using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class InitVariablesCodeGenerator<T> : ScriptCodeGenerator
    {
        public InitVariablesCodeGenerator(McMod mcMod, IEnumerable<T> elements = null) : base(mcMod) => Elements = elements;

        protected IEnumerable<T> Elements { get; }

        protected abstract string GetElementName(T element);

        protected static readonly char[] InvalidVariableNameChars = new char[] {
            '.', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '/', '\\', '|', '[', ']', '{', '}', ';', ':', '\'', '"', ',', '<', '>', '?', ' '
        };

        protected virtual CodeMemberField CreateElementField(T element)
        {
            string typeName = element.GetType().Name;
            string elementName = MakeVariableNameValid(GetElementName(element));
            CodeMemberField field = new CodeMemberField(typeName, elementName.ToUpper()) {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                InitExpression = new CodeObjectCreateExpression(typeName + "Base", new CodePrimitiveExpression(GetElementName(element)))
            };
            return field;
        }

        protected CodeCompileUnit CreateDefaultTargetCodeUnit(string className, string elementType)
        {
            CodeTypeDeclaration clas = NewClassWithMembers(className);

            string listVarName = null;
            if (ScriptLocator is InitClassLocator initLocator)
            {
                listVarName = initLocator.InitFieldName;
            }
            else
            {
                listVarName = elementType.ToUpper() + "S";
            }
            CodeMemberField listField = new CodeMemberField($"List<{elementType}>", listVarName) {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                InitExpression = new CodeObjectCreateExpression($"ArrayList<{elementType}>")
            };
            clas.Members.Add(listField);

            if (Elements != null)
            {
                foreach (T element in Elements)
                {
                    clas.Members.Add(CreateElementField(element));
                }
            }

            CodeNamespace package = NewPackage(SourceCodeLocator.Manager(Modname, Organization).PackageName, clas,
                                                     "java.util.ArrayList",
                                                     "java.util.List");
            return NewCodeUnit(package);
        }

        protected string MakeVariableNameValid(string varName)
        {
            char validChar = '_';
            if (char.IsDigit(varName[0]))
            {
                varName = validChar + varName;
            }
            foreach (char invalidChar in InvalidVariableNameChars)
            {
                varName = varName.Replace(invalidChar, validChar);
            }
            if (!JavaCodeGenerator.IsValidJavaIdentifier(varName))
            {
                string newElementName = varName;
                foreach (char varChar in varName)
                {
                    if (!System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier("" + validChar + varChar))
                    {
                        newElementName = varName.Replace(varChar, validChar);
                    }
                }
                varName = newElementName;
            }
            return varName;
        }
    }
}
