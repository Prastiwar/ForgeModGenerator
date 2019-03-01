using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public partial class JavaCodeGenerator : ICodeGenerator
    {
        private void GenerateSnippetStatement(CodeSnippetStatement e) => output.WriteLine(e.Value);

        private void GenerateStatement(CodeStatement e)
        {
            switch (e)
            {
                case CodeCommentStatement val:
                    GenerateCommentStatement(val);
                    break;
                case CodeMethodReturnStatement val:
                    GenerateMethodReturnStatement(val);
                    break;
                case CodeConditionStatement val:
                    GenerateConditionStatement(val);
                    break;
                case CodeTryCatchFinallyStatement val:
                    GenerateTryCatchFinallyStatement(val);
                    break;
                case CodeAssignStatement val:
                    GenerateAssignStatement(val);
                    break;
                case CodeExpressionStatement val:
                    GenerateExpressionStatement(val);
                    break;
                case CodeIterationStatement val:
                    GenerateIterationStatement(val);
                    break;
                case CodeThrowExceptionStatement val:
                    GenerateThrowExceptionStatement(val);
                    break;
                case CodeSnippetStatement val:
                    // Don't indent snippet statements, in order to preserve the column
                    // information from the original code. This improves the debugging experience
                    int savedIndent = Indent;
                    Indent = 0;
                    GenerateSnippetStatement(val);
                    Indent = savedIndent; // Restore the indent
                    break;
                case CodeVariableDeclarationStatement val:
                    GenerateVariableDeclarationStatement(val);
                    break;
                case CodeBreakLabelStatement val:
                    GenerateBreakLabelStatement(val);
                    break;
                case CodeContinueLabelStatement val:
                    GenerateContinueLabelStatement(val);
                    break;
                case CodeLabeledStatement val:
                    GenerateLabeledStatement(val);
                    break;
                case CodeForeachStatement val:
                    GenerateForEachStatement(val);
                    break;
                default:
                    throw new ArgumentException(InvalidElementType(e.GetType()));
            }
        }

        private void GenerateStatements(CodeStatementCollection stms)
        {
            IEnumerator en = stms.GetEnumerator();
            while (en.MoveNext())
            {
                ((ICodeGenerator)this).GenerateCodeFromStatement((CodeStatement)en.Current, output.InnerWriter, options);
            }
        }

        private void GenerateCommentStatement(CodeCommentStatement e)
        {
            if (e.Comment == null)
            {
                throw new ArgumentNullException("Comment can't be null " + e);
            }
            GenerateComment(e.Comment);
        }

        private void GenerateCommentStatements(CodeCommentStatementCollection e)
        {
            foreach (CodeCommentStatement comment in e)
            {
                GenerateCommentStatement(comment);
            }
        }

        private void GenerateConditionStatement(CodeConditionStatement e)
        {
            output.Write("if (");
            GenerateExpression(e.Condition);
            output.Write(")");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.TrueStatements);
            Indent--;

            CodeStatementCollection falseStatemetns = e.FalseStatements;
            if (falseStatemetns.Count > 0)
            {
                output.Write("}");
                if (options.ElseOnClosing)
                {
                    output.Write(" ");
                }
                else
                {
                    output.WriteLine("");
                }
                output.Write("else");
                OutputStartingBrace();
                Indent++;
                GenerateStatements(e.FalseStatements);
                Indent--;
            }
            output.WriteLine("}");
        }

        private void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
        {
            output.Write("throw");
            if (e.ToThrow != null)
            {
                output.Write(" ");
                GenerateExpression(e.ToThrow);
            }
            output.WriteLine(";");
        }

        private void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
        {
            output.Write("try");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.TryStatements);
            Indent--;
            CodeCatchClauseCollection catches = e.CatchClauses;
            if (catches.Count > 0)
            {
                IEnumerator en = catches.GetEnumerator();
                while (en.MoveNext())
                {
                    output.Write("}");
                    if (options.ElseOnClosing)
                    {
                        output.Write(" ");
                    }
                    else
                    {
                        output.WriteLine("");
                    }
                    CodeCatchClause current = (CodeCatchClause)en.Current;
                    output.Write("catch (");
                    OutputType(current.CatchExceptionType);
                    output.Write(" ");
                    OutputIdentifier(current.LocalName);
                    output.Write(")");
                    OutputStartingBrace();
                    Indent++;
                    GenerateStatements(current.Statements);
                    Indent--;
                }
            }

            CodeStatementCollection finallyStatements = e.FinallyStatements;
            if (finallyStatements.Count > 0)
            {
                output.Write("}");
                if (options.ElseOnClosing)
                {
                    output.Write(" ");
                }
                else
                {
                    output.WriteLine("");
                }
                output.Write("finally");
                OutputStartingBrace();
                Indent++;
                GenerateStatements(finallyStatements);
                Indent--;
            }
            output.WriteLine("}");
        }

        private void GenerateAssignStatement(CodeAssignStatement e)
        {
            GenerateExpression(e.Left);
            output.Write(" = ");
            GenerateExpression(e.Right);
            if (!generatingForLoop)
            {
                output.WriteLine(";");
            }
        }

        private void GenerateBreakLabelStatement(CodeBreakLabelStatement e)
        {
            output.Write("break ");
            output.Write(e.Label);
            output.WriteLine(";");
        }

        private void GenerateContinueLabelStatement(CodeContinueLabelStatement e)
        {
            output.Write("continue ");
            output.Write(e.Label);
            output.WriteLine(";");
        }

        private void GenerateLabeledStatement(CodeLabeledStatement e)
        {
            Indent--;
            output.Write(e.Label);
            output.WriteLine(":");
            Indent++;
            if (e.Statement != null)
            {
                GenerateStatement(e.Statement);
            }
        }

        private void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
        {
            OutputTypeNamePair(e.Type, e.Name);
            if (e.InitExpression != null)
            {
                output.Write(" = ");
                GenerateExpression(e.InitExpression);
            }
            if (!generatingForLoop)
            {
                output.WriteLine(";");
            }
        }

        private void GenerateExpressionStatement(CodeExpressionStatement e)
        {
            GenerateExpression(e.Expression);
            if (!generatingForLoop)
            {
                output.WriteLine(";");
            }
        }

        private void GenerateIterationStatement(CodeIterationStatement e)
        {
            generatingForLoop = true;
            output.Write("for (");
            GenerateStatement(e.InitStatement);
            output.Write("; ");
            GenerateExpression(e.TestExpression);
            output.Write("; ");
            GenerateStatement(e.IncrementStatement);
            output.Write(")");
            OutputStartingBrace();
            generatingForLoop = false;
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateForEachStatement(CodeForeachStatement e)
        {
            generatingForLoop = true;
            output.Write("for (");
            GenerateStatement(e.Element);
            output.Write(" : ");
            GenerateExpression(e.Iterator);
            output.Write(")");
            OutputStartingBrace();
            generatingForLoop = false;
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
        {
            output.Write("return");
            if (e.Expression != null)
            {
                output.Write(" ");
                GenerateExpression(e.Expression);
            }
            output.WriteLine(";");
        }
    }
}
