using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.CodeGeneration
{
    public abstract class InitVariablesCodeGenerator<T> : ScriptCodeGenerator
    {
        public InitVariablesCodeGenerator(Mod mod, IEnumerable<T> elements = null) : base(mod) => Elements = elements ?? GetElementsForMod(mod);

        protected IEnumerable<T> Elements { get; }

        protected abstract string GetElementName(T element);

        protected virtual IEnumerable<T> GetElementsForMod(Mod mod) => null;

        protected virtual CodeMemberField CreateElementField(T element)
        {
            string typeName = element.GetType().Name;
            CodeMemberField field = new CodeMemberField(typeName, GetElementName(element).Replace(' ', '_').ToUpper()) {
                Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
                InitExpression = new CodeObjectCreateExpression(typeName + "Base", new CodePrimitiveExpression(GetElementName(element)))
            };
            return field;
        }

        protected CodeCompileUnit CreateDefaultTargetCodeUnit(string className, string elementType, string baseFieldTypeCreation)
        {
            CodeTypeDeclaration clas = NewClassWithMembers(className);

            CodeMemberField listField = new CodeMemberField($"List<{elementType}>", elementType.ToUpper() + "s") {
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

            CodeNamespace package = NewPackage(clas, "java.util.ArrayList", "java.util.List", $"net.minecraft.util.{baseFieldTypeCreation}");
            return NewCodeUnit(package);
        }
    }
}
