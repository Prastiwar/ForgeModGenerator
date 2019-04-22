using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class JavaCodeMemberMethod : CodeMemberMethod
    {
        public JavaCodeMemberMethod() : base() => ThrowsExceptions = new List<string>();
        public JavaCodeMemberMethod(params string[] exceptions) => ThrowsExceptions = new List<string>(exceptions);
        public JavaCodeMemberMethod(List<string> exceptions) => ThrowsExceptions = exceptions ?? new List<string>();

        public List<string> ThrowsExceptions { get; }
    }
}
