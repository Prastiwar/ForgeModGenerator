using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public static class JavaCodeExtensions
    {
        public static bool IsInterface(this CodeTypeReference typeRef)
        {
            bool isInterface = (bool)typeof(CodeTypeReference).GetProperty("IsInterface", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(typeRef);
            // IsInterface property is false if not used ctor with Type, so
            // check if it's interface by naming convention
            if (!isInterface)
            {
                string baseType = typeRef.BaseType;
                return baseType.Length >= 2 && baseType[0] == 'I' && char.IsUpper(baseType[1]);
            }
            return isInterface;
        }

        public static void CallOutputTabs(this IndentedTextWriter writer) => 
            typeof(IndentedTextWriter).GetMethod("InternalOutputTabs", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(writer, null);
    }
}
