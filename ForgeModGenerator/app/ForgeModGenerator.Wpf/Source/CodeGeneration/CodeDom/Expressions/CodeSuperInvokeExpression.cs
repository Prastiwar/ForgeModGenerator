using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeSuperConstructorInvokeExpression : CodeMethodInvokeExpression
    {
        public CodeSuperConstructorInvokeExpression(params CodeExpression[] parameters) : base(null, "super", parameters ?? new CodeExpression[0]) { }

        public void AddVariableParameter(string variableName) => Parameters.Add(new CodeVariableReferenceExpression(variableName));
        public void AddParameter(object primitiveValue) => Parameters.Add(new CodePrimitiveExpression(primitiveValue));
    }
}
