using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.JavaCodeDom
{
    public class CodeSuperConstructorInvokeExpression : CodeMethodInvokeExpression
    {
        public CodeSuperConstructorInvokeExpression(params CodeExpression[] parameters) : base(null, "super", parameters) { }

        public void AddVariableParameter(string variableName) => Parameters.Add(new CodeVariableReferenceExpression(variableName));
        public void AddParameter(object primitiveValue) => Parameters.Add(new CodePrimitiveExpression(primitiveValue));
    }
}
