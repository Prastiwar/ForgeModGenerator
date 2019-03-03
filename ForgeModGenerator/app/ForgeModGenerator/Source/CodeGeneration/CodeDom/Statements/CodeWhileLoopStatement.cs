using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeWhileLoopStatement : CodeStatement
    {
        public CodeWhileLoopStatement(CodeExpression conditionExpression) => ConditionExpression = conditionExpression;

        public CodeStatementCollection Statements { get; } = new CodeStatementCollection();
        public CodeExpression ConditionExpression { get; set; }
    }
}
