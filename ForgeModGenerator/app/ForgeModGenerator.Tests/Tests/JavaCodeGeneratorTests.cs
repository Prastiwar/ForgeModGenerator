﻿using ForgeModGenerator.CodeGeneration.CodeDom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace ForgeModGenerator.Tests
{
    [TestClass]
    public class JavaCodeGeneratorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void IsInterface()
        {
            CodeTypeReference typeRef = new CodeTypeReference("IInterface");
            Assert.IsTrue(typeRef.IsInterface());

            typeRef = new CodeTypeReference(typeof(IDummyInterface));
            Assert.IsTrue(typeRef.IsInterface());

            typeRef = new CodeTypeReference(typeof(DummyInterface));
            Assert.IsTrue(typeRef.IsInterface());

            typeRef = new CodeTypeReference("Interface");
            Assert.IsFalse(typeRef.IsInterface());
        }

        [TestMethod]
        public void GenerateClass()
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace package = new CodeNamespace("testPackage");
            CodeTypeDeclaration typ = new CodeTypeDeclaration("TestClass") { IsClass = true };
            typ.BaseTypes.Add(new CodeTypeReference("ISomeInterface"));
            typ.BaseTypes.Add(new CodeTypeReference("ISomeOtherInterface"));
            typ.BaseTypes.Add(new CodeTypeReference("SomeClass"));
            package.Types.Add(typ);
            unit.Namespaces.Add(package);
            string code = GenerateCode(TestContext, unit);
            Assert.IsTrue(code.Contains(""), code);
        }

        [TestMethod]
        public void GenerateComment()
        {
            CodeMemberMethod method = new CodeMemberMethod() { Name = "someMethod" };
            CodeCommentStatement comment = new CodeCommentStatement("Does nothing, but something I should write there. \n So I write something like this {@code int} ", true);
            method.Comments.Add(comment);
            string code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains(@"
    /**
     * Does nothing, but something I should write there. 
     * So I write something like this {@code int} 
     */"), code);

            comment.Comment.DocComment = false;
            comment.Comment.Text = "Some other comment \n just to be happy";
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains(@"
    // Some other comment 
    // just to be happy"), code);
        }

        [TestMethod]
        public void GenerateField()
        {
            CodeMemberField field = new CodeMemberField(new CodeTypeReference(typeof(string)), "someField") { Attributes = MemberAttributes.Private };
            string code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("private String someField;"), code);

            field.Name = "someBlock";
            field.Type = new CodeTypeReference("Block");
            field.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
            code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("public static final Block someBlock;"), code);
            
            field.Attributes = MemberAttributes.Public | JavaMemberAttributes.StaticOnly;
            code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("public static Block someBlock;"), code);

            field.Attributes = MemberAttributes.FamilyAndAssembly | JavaMemberAttributes.StaticOnly;
            code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("protected static Block someBlock;"), code);

            field.Attributes = MemberAttributes.FamilyOrAssembly;
            code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("protected Block someBlock;"), code);

            field.Attributes = MemberAttributes.Assembly;
            code = GenerateMember(TestContext, field);
            Assert.IsTrue(code.Contains("Block someBlock;"), code);
        }

        [TestMethod]
        public void GenerateMethod()
        {
            CodeMemberMethod method = new CodeMemberMethod() { Name = "someMethod" };
            string code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("private final void someMethod() {"), code);

            method.Name = "someOtherMethod";
            method.Attributes = MemberAttributes.Public | JavaMemberAttributes.StaticOnly;
            method.ReturnType = new CodeTypeReference(typeof(int));
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("public static int someOtherMethod() {"), code);

            method.Name = "someYetOtherMethod";
            method.Attributes = MemberAttributes.Family | JavaMemberAttributes.StaticFinal;
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("protected static final int someYetOtherMethod() {"), code);

            method.Attributes = MemberAttributes.FamilyAndAssembly | JavaMemberAttributes.StaticFinal;
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("protected static final int someYetOtherMethod() {"), code);

            method.Attributes = MemberAttributes.FamilyOrAssembly | JavaMemberAttributes.StaticFinal;
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("protected static final int someYetOtherMethod() {"), code);

            method.Attributes = MemberAttributes.Abstract;
            method.ReturnType = new CodeTypeReference(typeof(void));
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("abstract void someYetOtherMethod();"), code);

            method.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("public abstract void someYetOtherMethod();"), code);

            method.Attributes = MemberAttributes.Abstract | MemberAttributes.Assembly;
            code = GenerateMember(TestContext, method);
            Assert.IsTrue(code.Contains("abstract void someYetOtherMethod();"), code);
        }

        [TestMethod]
        public void GenerateForEach()
        {
            CodeForeachStatement forEach = new CodeForeachStatement(new CodeVariableDeclarationStatement(typeof(int), "integer"), new CodeTypeReferenceExpression("someList"));
            string code = GenerateStatement(TestContext, forEach);
            Assert.IsTrue(code.Contains("for (int integer : someList) {"), code);

            forEach = new CodeForeachStatement(new CodeVariableDeclarationStatement("Block", "block"),
                                               new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ModBlocks"), "BLOCKS"));
            code = GenerateStatement(TestContext, forEach);
            Assert.IsTrue(code.Contains("for (Block block : ModBlocks.BLOCKS) {"), code);
        }

        public static string GenerateExpression(TestContext context, CodeExpression expression) => GenerateStatement(context, new CodeExpressionStatement(expression));
        public static string GenerateStatement(TestContext context, CodeStatement statement)
        {
            CodeMemberMethod testMethod = new CodeMemberMethod() { Name = "testMethod" };
            testMethod.Statements.Add(statement);
            return GenerateMember(context, testMethod);
        }

        public static string GenerateMember(TestContext context, CodeTypeMember member)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace package = new CodeNamespace("testPackage");
            CodeTypeDeclaration typ = new CodeTypeDeclaration("TestClass") { IsClass = true };
            typ.Members.Add(member);
            package.Types.Add(typ);
            unit.Namespaces.Add(package);
            return GenerateCode(context, unit);
        }

        public static string GenerateCode(TestContext context, CodeCompileUnit targetCodeUnit, CodeGeneratorOptions options = null)
        {
            string filePath = Path.Combine(context.TestResultsDirectory, "GeneratedCode.java");
            options = options ?? new CodeGeneratorOptions() { BracingStyle = "Block" };

            using (JavaCodeProvider javaProvider = new JavaCodeProvider())
            {
                using (StreamWriter sourceWriter = new StreamWriter(filePath))
                {
                    javaProvider.GenerateCodeFromCompileUnit(targetCodeUnit, sourceWriter, options);
                }
            }
            return File.ReadAllText(filePath);
        }
    }
}