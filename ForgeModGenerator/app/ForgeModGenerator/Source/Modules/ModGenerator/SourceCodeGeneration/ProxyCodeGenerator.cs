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
            string sourcePath = ModPaths.SourceCodeRootFolder(Modname, Organization);
            ScriptFilePaths = new string[] {
                Path.Combine(sourcePath, SourceCodeLocator.CommonProxyInterface.RelativePath),
                Path.Combine(sourcePath, SourceCodeLocator.ClientProxy.RelativePath),
                Path.Combine(sourcePath, SourceCodeLocator.ServerProxy.RelativePath)
            };
        }

        protected override string[] ScriptFilePaths { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.CommonProxyInterface.ClassName)
            {
                return CreateICommonProxyCodeUnit();
            }
            else if (fileName == SourceCodeLocator.ClientProxy.ClassName)
            {
                return CreateClientProxyCodeUnit();
            }
            else if (fileName == SourceCodeLocator.ServerProxy.ClassName)
            {
                return CreateServerProxyCodeUnit();
            }
            else
            {
                throw new NotImplementedException($"CodeCompileUnit for {fileName} not found");
            }
        }

        private CodeCompileUnit CreateClientProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ClientProxy.ClassName, SourceCodeLocator.CommonProxyInterface.ClassName);
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
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ServerProxy.ClassName, SourceCodeLocator.CommonProxyInterface.ClassName);
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(proxyClass, "net.minecraft.item.Item");
        }

        private CodeCompileUnit CreateICommonProxyCodeUnit() => NewCodeUnit(NewInterface(SourceCodeLocator.CommonProxyInterface.ClassName, CreateRegisterItemRendererMethod()), "net.minecraft.item.Item");

        private CodeMemberMethod CreateRegisterItemRendererMethod() => NewMethod("registerItemRenderer", typeof(void).FullName, MemberAttributes.Public, new Parameter("Item", "item"),
                                                                                                                                                         new Parameter(typeof(int).FullName, "meta"),
                                                                                                                                                         new Parameter(typeof(string).FullName, "id"));
    }
}
