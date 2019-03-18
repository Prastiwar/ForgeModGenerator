using System.CodeDom;
using System.Reflection;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    /// <summary>
    /// Defines member attribute identifiers for java class members 
    /// that are not possible with System.CodeDom.MemberAttributes
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public static class JavaAttributes
    {
        public const MemberAttributes StaticOnly = (MemberAttributes)15;
        public const MemberAttributes StaticFinal = (MemberAttributes)3; // convenient attribute for MemberAttributes.Statics
    }
}
