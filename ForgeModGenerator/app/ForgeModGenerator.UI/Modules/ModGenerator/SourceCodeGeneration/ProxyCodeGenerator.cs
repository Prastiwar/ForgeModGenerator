using ForgeModGenerator.CodeGeneration;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ProxyCodeGenerator : MultiScriptsCodeGenerator
    {
        public ProxyCodeGenerator(string modname, string organization) : base(modname, organization)
        {
            string proxyFolder = ModPaths.GeneratedProxyFolder(modname, organization);
            ScriptFilePaths = new string[] {
                Path.Combine(proxyFolder, "ICommonProxy.java"),
                Path.Combine(proxyFolder, "ClientProxy.java"),
                Path.Combine(proxyFolder, "ServerProxy.java")
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            switch (fileName)
            {
                case "ICommonProxy":
                    return CreateICommonProxyCodeUnit();
                case "ClientProxy":
                    return CreateClientProxyCodeUnit();
                case "ServerProxy":
                    return CreateServerProxyCodeUnit();
                default:
                    throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateClientProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = GetDefaultClass("ClientProxy", false);
            proxyClass.BaseTypes.Add(new CodeTypeReference("ICommonProxy"));
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            CodeObjectCreateExpression modelResourceLocation =
                new CodeObjectCreateExpression("ModelResourceLocation", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("item"), "getRegistryName", null), new CodeVariableReferenceExpression("id"));
            CodeMethodInvokeExpression setCustomModelResourceLocation =
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("ModelLoader"),
                                                "setCustomModelResourceLocation",
                                                new CodeVariableReferenceExpression("item"),
                                                new CodeVariableReferenceExpression("meta"),
                                                modelResourceLocation);
            registerItemRendererMethod.Statements.Add(setCustomModelResourceLocation);
            proxyClass.Members.Add(registerItemRendererMethod);
            CodeNamespace package = GetDefaultPackage(proxyClass,
                                                      "net.minecraft.client.renderer.block.model.ModelResourceLocation",
                                                      "net.minecraft.item.Item",
                                                      "net.minecraftforge.client.model.ModelLoader");
            return GetDefaultCodeUnit(package);
        }

        private CodeCompileUnit CreateServerProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = GetDefaultClass("ServerProxy", false);
            proxyClass.BaseTypes.Add(new CodeTypeReference("ICommonProxy"));
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            proxyClass.Members.Add(registerItemRendererMethod);
            CodeNamespace package = GetDefaultPackage(proxyClass, "net.minecraft.item.Item");
            return GetDefaultCodeUnit(package);
        }

        private CodeCompileUnit CreateICommonProxyCodeUnit()
        {
            CodeTypeDeclaration proxyInterface = GetDefaultInterface("ICommonProxy", false);
            proxyInterface.Members.Add(CreateRegisterItemRendererMethod());
            CodeNamespace package = GetDefaultPackage(proxyInterface, "net.minecraft.item.Item");
            return GetDefaultCodeUnit(package);
        }

        private CodeMemberMethod CreateRegisterItemRendererMethod()
        {
            CodeMemberMethod registerItemRenderer = new CodeMemberMethod() {
                Attributes = MemberAttributes.Public
            };
            registerItemRenderer.Parameters.Add(new CodeParameterDeclarationExpression("Item", "item"));
            registerItemRenderer.Parameters.Add(new CodeParameterDeclarationExpression("int", "meta"));
            registerItemRenderer.Parameters.Add(new CodeParameterDeclarationExpression("String", "id"));
            return registerItemRenderer;
        }
    }
}
