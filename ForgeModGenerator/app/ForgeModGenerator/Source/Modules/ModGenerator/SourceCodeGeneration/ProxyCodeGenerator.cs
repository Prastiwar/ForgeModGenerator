using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ProxyCodeGenerator : MultiScriptsCodeGenerator
    {
        public ProxyCodeGenerator(Mod mod) : base(mod)
        {
            string proxyFolder = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "proxy");
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
            CodeTypeDeclaration proxyClass = NewClassWithBases("ClientProxy", false, "ICommonProxy");
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            CodeObjectCreateExpression modelResourceLocation = NewObject("ModelResourceLocation", NewMethodInvokeVar("item", "getRegistryName", NewVarReference("id")));
            registerItemRendererMethod.Statements.Add(NewMethodInvokeType("ModelLoader", "setCustomModelResourceLocation", NewVarReference("item"), NewVarReference("meta"), modelResourceLocation));
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(proxyClass, "net.minecraft.client.renderer.block.model.ModelResourceLocation",
                                           "net.minecraft.item.Item",
                                           "net.minecraftforge.client.model.ModelLoader");
        }

        private CodeCompileUnit CreateServerProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = NewClassWithBases("ServerProxy", false, "ICommonProxy");
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(proxyClass, "net.minecraft.item.Item");
        }

        private CodeCompileUnit CreateICommonProxyCodeUnit() => NewCodeUnit(NewInterface("ICommonProxy", CreateRegisterItemRendererMethod()), "net.minecraft.item.Item");

        private CodeMemberMethod CreateRegisterItemRendererMethod() => NewMethod("registerItemRenderer", typeof(void).FullName, MemberAttributes.Public, new Parameter("Item", "item"),
                                                                                                                                                         new Parameter(typeof(int).FullName, "meta"),
                                                                                                                                                         new Parameter(typeof(string).FullName, "id"));
    }
}
