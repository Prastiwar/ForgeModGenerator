using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace ForgeModGenerator.CodeGeneration.JavaCodeDom
{
    public class JavaCodeGenerator : ICodeGenerator
    {
        public JavaCodeGenerator() { }
        public JavaCodeGenerator(IDictionary<string, string> providerOptions) => provOptions = providerOptions;

        private readonly IDictionary<string, string> provOptions;

        private static readonly string[] keywords = new string[] {
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

        private string FileExtension => ".java";

        public string CreateEscapedIdentifier(string value) => throw new System.NotImplementedException();
        public string CreateValidIdentifier(string value) => throw new System.NotImplementedException();
        public void GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o) => throw new System.NotImplementedException();
        public void GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o) => throw new System.NotImplementedException();
        public void GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o) => throw new System.NotImplementedException();
        public void GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o) => throw new System.NotImplementedException();
        public void GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o) => throw new System.NotImplementedException();
        public string GetTypeOutput(CodeTypeReference type) => throw new System.NotImplementedException();
        public bool IsValidIdentifier(string value) => throw new System.NotImplementedException();
        public bool Supports(GeneratorSupport supports) => throw new System.NotImplementedException();
        public void ValidateIdentifier(string value) => throw new System.NotImplementedException();
        public void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options) => throw new System.NotImplementedException();
    }
}