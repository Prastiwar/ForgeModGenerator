using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public partial class JavaCodeGenerator : ICodeGenerator
    {
        private void GenerateSnippetExpression(CodeSnippetExpression e) => output.Write(e.Value);
        private void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e) => output.Write("super");
        private void GenerateThisReferenceExpression(CodeThisReferenceExpression e) => output.Write("this");
        private void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e) => OutputIdentifier(e.ParameterName);
        private void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e) => OutputIdentifier(e.VariableName);
        private void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e) => OutputType(e.Type);

        private void GenerateExpression(CodeExpression e)
        {
            switch (e)
            {
                case CodeArrayCreateExpression val:
                    GenerateArrayCreateExpression(val);
                    break;
                case CodeBaseReferenceExpression val:
                    GenerateBaseReferenceExpression(val);
                    break;
                case CodeBinaryOperatorExpression val:
                    GenerateBinaryOperatorExpression(val);
                    break;
                case CodeCastExpression val:
                    GenerateCastExpression(val);
                    break;
                case CodeFieldReferenceExpression val:
                    GenerateFieldReferenceExpression(val);
                    break;
                case CodeArgumentReferenceExpression val:
                    GenerateArgumentReferenceExpression(val);
                    break;
                case CodeVariableReferenceExpression val:
                    GenerateVariableReferenceExpression(val);
                    break;
                case CodeArrayIndexerExpression val:
                    GenerateArrayIndexerExpression(val);
                    break;
                case CodeSnippetExpression val:
                    GenerateSnippetExpression(val);
                    break;
                case CodeMethodInvokeExpression val:
                    GenerateMethodInvokeExpression(val);
                    break;
                case CodeMethodReferenceExpression val:
                    GenerateMethodReferenceExpression(val);
                    break;
                case CodeObjectCreateExpression val:
                    GenerateObjectCreateExpression(val);
                    break;
                case CodeParameterDeclarationExpression val:
                    GenerateParameterDeclarationExpression(val);
                    break;
                case CodePrimitiveExpression val:
                    GeneratePrimitiveExpression(val);
                    break;
                case CodeThisReferenceExpression val:
                    GenerateThisReferenceExpression(val);
                    break;
                case CodeTypeReferenceExpression val:
                    GenerateTypeReferenceExpression(val);
                    break;
                case CodeInstanceOfExpression val:
                    GenerateInstanceOfExpression(val);
                    break;
                default:
                    if (e == null)
                    {
                        throw new ArgumentNullException(nameof(e));
                    }
                    else
                    {
                        throw new ArgumentException(InvalidElementType(e.GetType()));
                    }
            }
        }

        private void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
        {
            output.Write("new ");

            CodeExpressionCollection init = e.Initializers;
            if (init.Count > 0)
            {
                OutputType(e.CreateType);
                if (e.CreateType.ArrayRank == 0)
                {
                    // Unfortunately, many clients are already calling this without array
                    // types. This will allow new clients to correctly use the array type and
                    // not break existing clients. For VNext, stop doing this.
                    output.Write("[]");
                }
                output.WriteLine(" {");
                Indent++;
                OutputExpressionList(init, true);
                Indent--;
                output.Write("}");
            }
            else
            {
                output.Write(GetBaseTypeOutput(e.CreateType));
                output.Write("[");
                if (e.SizeExpression != null)
                {
                    GenerateExpression(e.SizeExpression);
                }
                else
                {
                    output.Write(e.Size);
                }
                output.Write("]");
                CodeTypeReference arrayElementType = e.CreateType.ArrayElementType;
                while (arrayElementType != null)
                {
                    output.Write("[]");
                    arrayElementType = arrayElementType.ArrayElementType;
                }
            }
        }

        private void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
        {
            GenerateExpression(e.TargetObject);
            output.Write("[");
            GenerateExpression(e.Indices[0]);
            output.Write("]");
        }

        private void GenerateInstanceOfExpression(CodeInstanceOfExpression e)
        {
            output.Write("instanceof ");
            OutputType(e.Type);
        }

        private void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                // Parameter attributes should be in-line for readability
                GenerateAttributes(e.CustomAttributes, null, true);
            }
            OutputTypeNamePair(e.Type, e.Name);
        }

        private void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
        {
            GenerateMethodReferenceExpression(e.Method);
            output.Write("(");
            OutputExpressionList(e.Parameters);
            output.Write(")");
        }

        private void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                if (e.TargetObject is CodeBinaryOperatorExpression)
                {
                    output.Write("(");
                    GenerateExpression(e.TargetObject);
                    output.Write(")");
                }
                else
                {
                    GenerateExpression(e.TargetObject);
                }
                output.Write(".");
            }
            OutputIdentifier(e.MethodName);
            if (e.TypeArguments.Count > 0)
            {
                output.Write(GetTypeArgumentsOutput(e.TypeArguments));
            }
        }

        private void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                GenerateExpression(e.TargetObject);
                output.Write(".");
            }
            OutputIdentifier(e.FieldName);
        }

        private void GenerateCastExpression(CodeCastExpression e)
        {
            output.Write("((");
            OutputType(e.TargetType);
            output.Write(")(");
            GenerateExpression(e.Expression);
            output.Write("))");
        }

        private void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
        {
            bool indentedExpression = false;
            output.Write("(");

            GenerateExpression(e.Left);
            output.Write(" ");

            if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression)
            {
                // In case the line gets too long with nested binary operators, we need to output them on
                // different lines. However we want to indent them to maintain readability, but this needs
                // to be done only once;
                if (!inNestedBinary)
                {
                    indentedExpression = true;
                    inNestedBinary = true;
                    Indent += 3;
                }
                ContinueOnNewLine("");
            }

            OutputOperator(e.Operator);

            output.Write(" ");
            GenerateExpression(e.Right);

            output.Write(")");
            if (indentedExpression)
            {
                Indent -= 3;
                inNestedBinary = false;
            }
        }

        private void GeneratePrimitiveExpression(CodePrimitiveExpression e)
        {
            switch (e.Value)
            {
                case null:
                    output.Write(NullToken);
                    break;
                case char val:
                    GeneratePrimitiveChar(val);
                    break;
                case string val:
                    output.Write(QuoteSnippetString(val));
                    break;
                case byte val:
                    output.Write(val.ToString(CultureInfo.InvariantCulture));
                    break;
                case short val:
                    output.Write(val.ToString(CultureInfo.InvariantCulture));
                    break;
                case int val:
                    output.Write(val.ToString(CultureInfo.InvariantCulture));
                    break;
                case long val:
                    output.Write(val.ToString(CultureInfo.InvariantCulture));
                    break;
                case float val:
                    GenerateSingleFloatValue(val);
                    break;
                case double val:
                    GenerateDoubleValue(val);
                    break;
                case bool val:
                    output.Write(val ? "true" : "false");
                    break;
                default:
                    throw new ArgumentException("Invalid Primitive Type " + e.Value.GetType());
            }
        }

        private void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
        {
            output.Write("new ");
            OutputType(e.CreateType);
            output.Write("(");
            OutputExpressionList(e.Parameters);
            output.Write(")");
        }
    }
}
