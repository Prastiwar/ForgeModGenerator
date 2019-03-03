using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeSimpleForLoopStatement : CodeStatement
    {
        public CodeSimpleForLoopStatement(CodeVariableDeclarationStatement indexVariable, CodeExpression lengthExpression,  bool loopBackwards = false)
        {
            LengthExpression = lengthExpression;
            IndexVariable = indexVariable;
            LoopBackwards = loopBackwards;
        }

        public CodeStatementCollection Statements { get; } = new CodeStatementCollection();
        public CodeExpression LengthExpression { get; set; }
        public CodeVariableDeclarationStatement IndexVariable { get; set; }
        public bool LoopBackwards { get; set; }
    }
}
