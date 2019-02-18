using System.CodeDom;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public class CodeForeachStatement : CodeStatement
    {
        public CodeForeachStatement() { }

        public CodeForeachStatement(CodeVariableDeclarationStatement element, CodeExpression iterator, params CodeStatement[] statements)
        {
            if (element.InitExpression != null)
            {
                throw new System.InvalidOperationException($"{nameof(element)} cannot be initialized in foreach loop");
            }
            Element = element;
            Iterator = iterator;
            if (statements != null)
            {
                Statements.AddRange(statements);
            }
        }

        public CodeVariableDeclarationStatement Element { get; set; }
        public CodeExpression Iterator { get; set; }
        public CodeStatementCollection Statements { get; } = new CodeStatementCollection();
    }
}
