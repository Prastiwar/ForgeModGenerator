using System;
using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeInstanceOfExpression : CodeExpression
    {
        public CodeInstanceOfExpression() { }

        public CodeInstanceOfExpression(CodeTypeReference type) => Type = type;

        public CodeInstanceOfExpression(string type) => Type = new CodeTypeReference(type);

        public CodeInstanceOfExpression(Type type) => Type = new CodeTypeReference(type);

        private CodeTypeReference type;
        public CodeTypeReference Type {
            get => type ?? (type = new CodeTypeReference(""));
            set => type = value;
        }
    }
}
