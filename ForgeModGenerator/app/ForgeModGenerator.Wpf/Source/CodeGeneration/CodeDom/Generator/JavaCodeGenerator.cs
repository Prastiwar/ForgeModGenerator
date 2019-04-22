using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ForgeModGenerator.CodeGeneration.CodeDom
{
    public sealed partial class JavaCodeGenerator : ICodeGenerator, IDisposable
    {
        public JavaCodeGenerator() { }

        private const int ParameterMultilineThreshold = 15;
        private const int MaxLineLength = 80;

        private IndentedTextWriter output;
        private CodeGeneratorOptions options;

        private bool inNestedBinary = false;
        private bool generatingForLoop = false;

        private CodeTypeDeclaration currentClass;
        private CodeTypeMember currentMember;
        private string CurrentTypeName => currentClass != null ? currentClass.Name : "<% unknown %>";
        private bool IsCurrentInterface => currentClass != null ? currentClass.IsInterface : false;
        private bool IsCurrentClass => currentClass != null ? currentClass.IsClass : false;
        private bool IsCurrentEnum => currentClass != null ? currentClass.IsEnum : false;

        private string NullToken => "null";

        private int Indent {
            get => output.Indent;
            set => output.Indent = value;
        }

        private static readonly HashSet<string> keywords = new HashSet<string> {
            // 1 character
            null,
            // 2 characters
            "do", "if",
            // 3 characters
            "for",  "int", "new", "try",
            // 4 characters
            "byte", "case", "char", "else", "enum", "goto", "long", "null", "this", "true", "void",
            // 5 characters
            "break", "catch", "class", "const", "false", "float", "short", "throw", "while", "super", "final",
            // 6 characters
            "double", "Object", "public", "return", "sealed", "static", "String", "switch", "import", "assert", "native", "throws",
            // 7 characters
            "default", "finally", "private", "virtual", "boolean", "extends",
            // 8 characters
            "abstract", "continue", "override", "readonly", "volatile", "strictfp",
            // 9 characters
            "interface", "protected", "transient",
            // 10 characters
            "instanceof", "implements",
            // 12 characters
            "synchronized",
        };

        private const GeneratorSupport LanguageSupport = GeneratorSupport.ArraysOfArrays
                                                         | GeneratorSupport.EntryPointMethod
                                                         | GeneratorSupport.StaticConstructors
                                                         | GeneratorSupport.TryCatchStatements
                                                         | GeneratorSupport.DeclareEnums
                                                         | GeneratorSupport.DeclareInterfaces
                                                         | GeneratorSupport.ChainedConstructorArguments
                                                         | GeneratorSupport.NestedTypes
                                                         | GeneratorSupport.MultipleInterfaceMembers
                                                         | GeneratorSupport.PublicStaticMembers
                                                         | GeneratorSupport.ComplexExpressions
                                                         | GeneratorSupport.GenericTypeReference
                                                         | GeneratorSupport.GenericTypeDeclaration;

        private readonly Dictionary<CodeBinaryOperatorType, string> binaryOperatorChars = new Dictionary<CodeBinaryOperatorType, string>() {
            { CodeBinaryOperatorType.Add, "+" },
            { CodeBinaryOperatorType.Subtract, "-" },
            { CodeBinaryOperatorType.Multiply, "*" },
            { CodeBinaryOperatorType.Divide , "/" },
            { CodeBinaryOperatorType.Modulus, "%" },
            { CodeBinaryOperatorType.Assign, "=" },
            { CodeBinaryOperatorType.IdentityInequality, "!=" },
            { CodeBinaryOperatorType.IdentityEquality, "==" },
            { CodeBinaryOperatorType.ValueEquality, "==" },
            { CodeBinaryOperatorType.BitwiseOr, "|" },
            { CodeBinaryOperatorType.BitwiseAnd, "&" },
            { CodeBinaryOperatorType.BooleanOr, "||" },
            { CodeBinaryOperatorType.BooleanAnd, "&&" },
            { CodeBinaryOperatorType.LessThan, "<" },
            { CodeBinaryOperatorType.LessThanOrEqual, "<=" },
            { CodeBinaryOperatorType.GreaterThan, ">" },
            { CodeBinaryOperatorType.GreaterThanOrEqual, ">=" }
        };

        private static bool IsKeyword(string value) => keywords.Contains(value);

        private string InvalidIndentifier(string value) => $"Indentifier is not valid: {value}";
        private string InvalidElementType(Type type) => $"InvalidElementType {type.FullName}";

        private bool GetUserData(CodeObject e, string property, bool defaultValue) => e.UserData[property] != null && e.UserData[property] is bool boolVal ? boolVal : defaultValue;
        private bool IsPrefixTwoUnderscore(string value) => value.Length < 3 ? false : (value[0] == '_') && (value[1] == '_') && (value[2] != '_');

        public static bool IsValidJavaIdentifier(string value)
        {
            // identifiers must be 1 char or longer
            if (value == null || value.Length == 0 || value.Length > 512)
            {
                return false;
            }

            // identifiers cannot be a keyword, unless they are escaped with an '$'
            if (value[0] != '$')
            {
                if (IsKeyword(value))
                {
                    return false;
                }
            }
            else
            {
                value = value.Substring(1);
            }
            return CodeGenerator.IsValidLanguageIndependentIdentifier(value);
        }

        public bool Supports(GeneratorSupport support) => ((support & LanguageSupport) == support);

        // Any identifier started with two consecutive underscores are reserved
        public string CreateEscapedIdentifier(string name) => IsKeyword(name) || IsPrefixTwoUnderscore(name) ? "$" + name : name;

        public bool IsValidIdentifier(string value) => IsValidJavaIdentifier(value);

        public void ValidateIdentifier(string value)
        {
            if (!IsValidIdentifier(value))
            {
                throw new ArgumentException(InvalidIndentifier(value));
            }
        }

        public string CreateValidIdentifier(string name)
        {
            if (IsPrefixTwoUnderscore(name))
            {
                name = "_" + name;
            }

            while (IsKeyword(name))
            {
                name = "_" + name;
            }
            return name;
        }

        private void ContinueOnNewLine(string st) => output.WriteLine(st);

        private void GenerateSnippetMember(CodeSnippetTypeMember e) => output.Write(e.Text);
        private void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e) => output.WriteLine(e.Value);

        private void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
        {
            if (!IsCurrentClass)
            {
                return;
            }

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            OutputMemberAccessModifier(e.Attributes);
            OutputIdentifier(CurrentTypeName);
            output.Write("(");
            OutputParameters(e.Parameters);
            output.Write(")");

            OutputStartingBrace();
            Indent++;
            CodeExpressionCollection thisArgs = e.ChainedConstructorArgs;
            if (thisArgs.Count > 0)
            {
                output.Write("this(");
                OutputExpressionList(thisArgs);
                output.Write(");");
            }
            GenerateStatements(e.Statements);
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateConstructors(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeConstructor codeConstructor)
                {
                    currentMember = (CodeTypeMember)en.Current;

                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    GenerateCommentStatements(currentMember.Comments);
                    GenerateConstructor(codeConstructor, e);
                }
            }
        }

        private void GenerateTypeConstructor(CodeTypeConstructor e)
        {
            if (!IsCurrentClass)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            output.Write("static ");
            output.Write(CurrentTypeName);
            output.Write("()");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateType(CodeTypeDeclaration e)
        {
            currentClass = e;
            GenerateCommentStatements(e.Comments);
            GenerateTypeStart(e);

            if (options.VerbatimOrder)
            {
                foreach (CodeTypeMember member in e.Members)
                {
                    GenerateTypeMember(member, e);
                }
            }
            else
            {
                GenerateFields(e);
                GenerateSnippetMembers(e);
                GenerateTypeConstructors(e);
                GenerateConstructors(e);
                GenerateMethods(e);
                GenerateNestedTypes(e);
            }
            currentClass = e; // Nested types clobber the current class, so reset it.
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateTypes(CodeNamespace e)
        {
            foreach (CodeTypeDeclaration c in e.Types)
            {
                if (options.BlankLinesBetweenMembers)
                {
                    output.WriteLine();
                }
                ((ICodeGenerator)this).GenerateCodeFromType(c, output.InnerWriter, options);
            }
        }

        private void GenerateTypeStart(CodeTypeDeclaration e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            OutputTypeAttributes(e);
            OutputIdentifier(e.Name);
            OutputTypeParameters(e.TypeParameters);

            bool first = true;
            CodeTypeReferenceCollection classes = new CodeTypeReferenceCollection();
            CodeTypeReferenceCollection interfaces = new CodeTypeReferenceCollection();

            foreach (CodeTypeReference typeRef in e.BaseTypes)
            {
                if (typeRef.IsInterface())
                {
                    interfaces.Add(typeRef);
                }
                else
                {
                    classes.Add(typeRef);
                }
            }

            foreach (CodeTypeReference typeClass in classes)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                else
                {
                    output.Write(" extends ");
                    first = false;
                }
                OutputType(typeClass);
            }

            first = true;
            foreach (CodeTypeReference typeInterface in interfaces)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                else
                {
                    output.Write(" implements ");
                    first = false;
                }
                OutputType(typeInterface);
            }
            OutputStartingBrace();
            Indent++;
        }

        private void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
        {
            if (options.BlankLinesBetweenMembers)
            {
                output.WriteLine();
            }

            if (member is CodeTypeDeclaration typeDeclaration)
            {
                ((ICodeGenerator)this).GenerateCodeFromType(typeDeclaration, output.InnerWriter, options);
                currentClass = declaredType; // Nested types clobber the current class, so reset it.
                return;
            }

            GenerateCommentStatements(member.Comments);

            if (member is CodeMemberField fieldMember)
            {
                GenerateField(fieldMember);
            }
            else if (member is CodeMemberMethod methodMember)
            {
                switch (member)
                {
                    case CodeConstructor val:
                        GenerateConstructor(val, declaredType);
                        break;
                    case CodeTypeConstructor val:
                        GenerateTypeConstructor(val);
                        break;
                    case CodeEntryPointMethod val:
                        GenerateEntryPointMethod(val, declaredType);
                        break;
                    default:
                        GenerateMethod(methodMember, declaredType);
                        break;
                }
            }
            else if (member is CodeSnippetTypeMember snippetMember)
            {
                // Don't indent snippets, in order to preserve the column
                // information from the original code.  This improves the debugging
                // experience.
                int savedIndent = Indent;
                Indent = 0;
                GenerateSnippetMember(snippetMember);
                Indent = savedIndent; // Restore the indent

                // Generate an extra new line at the end of the snippet.
                // If the snippet is comment and this type only contains comments.
                // The generated code will not compile.
                output.WriteLine();
            }
        }

        private void GenerateTypeConstructors(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeTypeConstructor typeConstructor)
                {
                    currentMember = (CodeTypeMember)en.Current;

                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    GenerateCommentStatements(currentMember.Comments);
                    GenerateTypeConstructor(typeConstructor);
                }
            }
        }

        private void GenerateSnippetMembers(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            bool hasSnippet = false;
            while (en.MoveNext())
            {
                if (en.Current is CodeSnippetTypeMember snippetMember)
                {
                    hasSnippet = true;
                    currentMember = (CodeTypeMember)en.Current;

                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    GenerateCommentStatements(currentMember.Comments);

                    // Don't indent snippets, in order to preserve the column
                    // information from the original code.  This improves the debugging
                    // experience.
                    int savedIndent = Indent;
                    Indent = 0;
                    GenerateSnippetMember(snippetMember);
                    Indent = savedIndent; // Restore the indent
                }
            }
            // Generate an extra new line at the end of the snippet.
            // If the snippet is comment and this type only contains comments.
            // The generated code will not compile.
            if (hasSnippet)
            {
                output.WriteLine();
            }
        }

        private void GenerateNestedTypes(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeTypeDeclaration typeDeclaration)
                {
                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    ((ICodeGenerator)this).GenerateCodeFromType(typeDeclaration, output.InnerWriter, options);
                }
            }
        }

        private void GenerateComment(CodeComment e)
        {
            string commentLineStart = e.DocComment ? " *" : "//";
            if (e.DocComment)
            {
                output.WriteLine("/**");
            }
            output.Write(commentLineStart + " ");

            string value = e.Text;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\u0000')
                {
                    continue;
                }
                output.Write(value[i]);

                if (value[i] == '\r')
                {
                    if (i < value.Length - 1 && value[i + 1] == '\n')
                    { // if next char is '\n', skip it
                        output.Write('\n');
                        i++;
                    }
                    output.CallOutputTabs();
                    output.Write(commentLineStart);
                }
                else if (value[i] == '\n')
                {
                    output.CallOutputTabs();
                    output.Write(commentLineStart);
                }
                else if (value[i] == '\u2028' || value[i] == '\u2029' || value[i] == '\u0085')
                {
                    output.Write(commentLineStart);
                }
            }
            output.WriteLine();
            if (e.DocComment)
            {
                output.WriteLine(" */");
            }
        }

        private void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
        {
            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            output.Write("public static ");
            OutputType(e.ReturnType);
            output.Write(" main");
            output.Write("(");
            OutputParameters(e.Parameters);
            output.Write(")");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            output.WriteLine("}");
        }

        private void GenerateMethods(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberMethod
                    && !(en.Current is CodeTypeConstructor)
                    && !(en.Current is CodeConstructor))
                {
                    currentMember = (CodeTypeMember)en.Current;

                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    GenerateCommentStatements(currentMember.Comments);
                    CodeMemberMethod imp = (CodeMemberMethod)en.Current;
                    if (en.Current is CodeEntryPointMethod entryPointMethod)
                    {
                        GenerateEntryPointMethod(entryPointMethod, e);
                    }
                    else
                    {
                        GenerateMethod(imp, e);
                    }
                }
            }
        }

        private void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
        {
            if (!(IsCurrentClass || IsCurrentInterface))
            {
                return;
            }

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            if (e.ReturnTypeCustomAttributes.Count > 0)
            {
                GenerateAttributes(e.ReturnTypeCustomAttributes);
            }

            if (!IsCurrentInterface)
            {
                if (e.PrivateImplementationType == null)
                {
                    OutputMemberAccessModifier(e.Attributes);
                    OutputVTableModifier(e.Attributes);
                    OutputMemberScopeModifier(e.Attributes);
                }
            }
            else
            {
                OutputVTableModifier(e.Attributes); // interfaces still need "new"
            }
            OutputType(e.ReturnType);
            output.Write(" ");
            if (e.PrivateImplementationType != null)
            {
                output.Write(GetBaseTypeOutput(e.PrivateImplementationType));
                output.Write(".");
            }
            OutputIdentifier(e.Name);
            OutputTypeParameters(e.TypeParameters);
            output.Write("(");
            OutputParameters(e.Parameters);
            output.Write(")");

            if (e is JavaCodeMemberMethod je)
            {
                if (je.ThrowsExceptions.Any())
                {
                    output.Write(" throws ");
                    int lastIndex = je.ThrowsExceptions.Count - 1;
                    for (int i = 0; i < je.ThrowsExceptions.Count; i++)
                    {
                        output.Write(je.ThrowsExceptions[i]);
                        if (i < lastIndex)
                        {
                            output.Write(", ");
                        }
                    }
                }
            }

            if (!IsCurrentInterface && (e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract)
            {
                OutputStartingBrace();
                Indent++;
                GenerateStatements(e.Statements);
                Indent--;
                output.WriteLine("}");
            }
            else
            {
                output.WriteLine(";");
            }
        }

        private void GenerateFields(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberField memberField)
                {
                    currentMember = (CodeTypeMember)en.Current;

                    if (options.BlankLinesBetweenMembers)
                    {
                        output.WriteLine();
                    }
                    GenerateCommentStatements(currentMember.Comments);
                    GenerateField(memberField);
                }
            }
        }

        private void GenerateField(CodeMemberField e)
        {
            if (IsCurrentInterface)
            {
                return;
            }

            if (IsCurrentEnum)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    GenerateAttributes(e.CustomAttributes);
                }
                OutputIdentifier(e.Name);
                if (e.InitExpression != null)
                {
                    output.Write(" = ");
                    GenerateExpression(e.InitExpression);
                }
                output.WriteLine(",");
            }
            else
            {
                if (e.CustomAttributes.Count > 0)
                {
                    GenerateAttributes(e.CustomAttributes);
                }
                OutputMemberAccessModifier(e.Attributes);
                OutputVTableModifier(e.Attributes);
                OutputFieldScopeModifier(e.Attributes);
                OutputTypeNamePair(e.Type, e.Name);
                if (e.InitExpression != null)
                {
                    output.Write(" = ");
                    GenerateExpression(e.InitExpression);
                }
                output.WriteLine(";");
            }
        }

        private void GeneratePrimitiveChar(char c)
        {
            output.Write('\'');
            switch (c)
            {
                case '\r':
                    output.Write("\\r");
                    break;
                case '\t':
                    output.Write("\\t");
                    break;
                case '\"':
                    output.Write("\\\"");
                    break;
                case '\'':
                    output.Write("\\\'");
                    break;
                case '\\':
                    output.Write("\\\\");
                    break;
                case '\0':
                    output.Write("\\0");
                    break;
                case '\n':
                    output.Write("\\n");
                    break;
                case '\u2028':
                case '\u2029':
                case '\u0084':
                case '\u0085':
                    AppendEscapedChar(null, c);
                    break;

                default:
                    if (char.IsSurrogate(c))
                    {
                        AppendEscapedChar(null, c);
                    }
                    else
                    {
                        output.Write(c);
                    }
                    break;
            }
            output.Write('\'');
        }

        private void AppendEscapedChar(StringBuilder b, char value)
        {
            if (b == null)
            {
                output.Write("\\u");
                output.Write(((int)value).ToString("X4", CultureInfo.InvariantCulture));
            }
            else
            {
                b.Append("\\u");
                b.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        /// Provides conversion to C-style formatting with escape codes.
        private string QuoteSnippetString(string value)
        {
            StringBuilder b = new StringBuilder(value.Length + 5);
            b.Append("\"");

            int i = 0;
            while (i < value.Length)
            {
                switch (value[i])
                {
                    case '\r':
                        b.Append("\\r");
                        break;
                    case '\t':
                        b.Append("\\t");
                        break;
                    case '\"':
                        b.Append("\\\"");
                        break;
                    case '\'':
                        b.Append("\\\'");
                        break;
                    case '\\':
                        b.Append("\\\\");
                        break;
                    case '\0':
                        b.Append("\\0");
                        break;
                    case '\n':
                        b.Append("\\n");
                        break;
                    case '\u2028':
                    case '\u2029':
                        AppendEscapedChar(b, value[i]);
                        break;
                    default:
                        b.Append(value[i]);
                        break;
                }

                if (i > 0 && i % MaxLineLength == 0)
                {
                    // If current character is a high surrogate and the following
                    // character is a low surrogate, don't break them.
                    // Otherwise when we write the string to a file, we might lose
                    // the characters.
                    if (char.IsHighSurrogate(value[i])
                        && (i < value.Length - 1)
                        && char.IsLowSurrogate(value[i + 1]))
                    {
                        b.Append(value[++i]);
                    }

                    b.Append("\" +");
                    b.Append(Environment.NewLine);
                    Indentation indentObj = new Indentation(output, Indent + 1);
                    b.Append(indentObj.IndentationString);
                    b.Append('\"');
                }
                ++i;
            }
            b.Append("\"");
            return b.ToString();
        }

        private void GenerateSingleFloatValue(float s)
        {
            if (float.IsNaN(s))
            {
                output.Write("Float.NaN");
            }
            else if (float.IsNegativeInfinity(s))
            {
                output.Write("Float.NEGATIVE_INFINITY");
            }
            else if (float.IsPositiveInfinity(s))
            {
                output.Write("Float.POSITIVE_INFINITY");
            }
            else
            {
                output.Write(s.ToString(CultureInfo.InvariantCulture));
                output.Write('F');
            }
        }

        private void GenerateDoubleValue(double d)
        {
            if (double.IsNaN(d))
            {
                output.Write("Double.NaN");
            }
            else if (double.IsNegativeInfinity(d))
            {
                output.Write("Double.NEGATIVE_INFINITY");
            }
            else if (double.IsPositiveInfinity(d))
            {
                output.Write("Double.POSITIVE_INFINITY");
            }
            else
            {
                output.Write(d.ToString("R", CultureInfo.InvariantCulture));
                output.Write("D");
            }
        }

        private void GenerateNamespaceStart(CodeNamespace e)
        {
            if (e.Name != null && e.Name.Length > 0)
            {
                output.Write("package ");
                string[] names = e.Name.Split('.');
                Debug.Assert(names.Length > 0);
                OutputIdentifier(names[0]);
                for (int i = 1; i < names.Length; i++)
                {
                    output.Write(".");
                    OutputIdentifier(names[i]);
                }
                output.Write(";");
                output.WriteLine();
                output.WriteLine();
            }
        }

        private void GenerateNamespace(CodeNamespace e)
        {
            GenerateCommentStatements(e.Comments);
            GenerateNamespaceStart(e);
            if (GetUserData(e, "GenerateImports", true))
            {
                GenerateNamespaceImports(e);
            }
            output.WriteLine("");
            GenerateTypes(e);
        }

        private void GenerateNamespaceImports(CodeNamespace e)
        {
            IEnumerator en = e.Imports.GetEnumerator();
            while (en.MoveNext())
            {
                GenerateNamespaceImport((CodeNamespaceImport)en.Current);
            }
        }

        private void GenerateNamespaceImport(CodeNamespaceImport e) => GenerateNamespaceImport(e.Namespace);

        private void GenerateNamespaceImport(string e)
        {
            output.Write("import ");
            OutputIdentifier(e);
            output.WriteLine(";");
        }

        private void GenerateNamespaces(CodeCompileUnit e)
        {
            foreach (CodeNamespace n in e.Namespaces)
            {
                ((ICodeGenerator)this).GenerateCodeFromNamespace(n, output.InnerWriter, options);
            }
        }

        private void GenerateCompileUnit(CodeCompileUnit e)
        {
            GenerateCompileUnitStart(e);
            GenerateNamespaces(e);
        }

        private void GenerateCompileUnitStart(CodeCompileUnit e)
        {
            if (GetUserData(e, SharedUserData.GenerateWarningMessage, true))
            {
                OutputGenerationMessage();
            }

            SortedList importList = new SortedList(StringComparer.Ordinal);
            foreach (CodeNamespace nspace in e.Namespaces)
            {
                if (string.IsNullOrEmpty(nspace.Name))
                {
                    // mark the namespace to stop it generating its own import list
                    nspace.UserData["GenerateImports"] = false;

                    // Collect the unique list of imports
                    foreach (CodeNamespaceImport import in nspace.Imports)
                    {
                        if (!importList.Contains(import.Namespace))
                        {
                            importList.Add(import.Namespace, import.Namespace);
                        }
                    }
                }
            }

            foreach (string import in importList.Keys)
            {
                GenerateNamespaceImport(import);
            }

            if (importList.Keys.Count > 0)
            {
                output.WriteLine("");
            }
        }

        private void Generate<T>(T e, TextWriter w, CodeGeneratorOptions o, Action<T> generationAction) where T : CodeObject
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                options = o ?? new CodeGeneratorOptions();
                output = new IndentedTextWriter(w, options.IndentString);
            }

            try
            {
                generationAction(e);
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    options = null;
                }
            }
        }

        void ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o) => Generate(e, w, o, GenerateExpression);
        void ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o) => Generate(e, w, o, GenerateStatement);
        void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o) => Generate(e, w, o, GenerateType);
        void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o) => Generate(e, w, o, GenerateNamespace);

        void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            if (e is CodeSnippetCompileUnit eSnippet)
            {
                Generate(eSnippet, w, o, GenerateSnippetCompileUnit);
            }
            else
            {
                Generate(e, w, o, GenerateCompileUnit);
            }
        }

        public void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
        {
            if (output != null)
            {
                throw new InvalidOperationException();
            }
            this.options = options ?? new CodeGeneratorOptions();
            output = new IndentedTextWriter(writer, this.options.IndentString);

            try
            {
                CodeTypeDeclaration dummyClass = new CodeTypeDeclaration();
                currentClass = dummyClass;
                GenerateTypeMember(member, dummyClass);
            }
            finally
            {
                currentClass = null;
                output = null;
                this.options = null;
            }
        }

        public void Dispose() => output.Dispose();
    }
}