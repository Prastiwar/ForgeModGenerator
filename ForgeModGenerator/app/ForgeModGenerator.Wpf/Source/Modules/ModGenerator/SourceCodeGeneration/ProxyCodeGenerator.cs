using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ProxyCodeGenerator : MultiScriptsCodeGenerator
    {
        public ProxyCodeGenerator(Mod mod) : base(mod) =>
            ScriptLocators = new ClassLocator[] {
                SourceCodeLocator.CommonProxyInterface(Modname, Organization),
                SourceCodeLocator.ClientProxy(Modname, Organization),
                SourceCodeLocator.ServerProxy(Modname, Organization)
            };

        public override ClassLocator[] ScriptLocators { get; }

        public override ClassLocator ScriptLocator => ScriptLocators[0];

        protected override CodeCompileUnit CreateTargetCodeUnit(string scriptPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);
            if (fileName == SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName)
            {
                return CreateICommonProxyCodeUnit();
            }
            else if (fileName == SourceCodeLocator.ClientProxy(Modname, Organization).ClassName)
            {
                return CreateClientProxyCodeUnit();
            }
            else if (fileName == SourceCodeLocator.ServerProxy(Modname, Organization).ClassName)
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
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ClientProxy(Modname, Organization).ClassName, SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName);
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            CodeObjectCreateExpression modelResourceLocation = NewObject("ModelResourceLocation", NewMethodInvokeVar("item", "getRegistryName"), NewVarReference("id"));
            registerItemRendererMethod.Statements.Add(NewMethodInvokeType("ModelLoader", "setCustomModelResourceLocation", NewVarReference("item"), NewVarReference("meta"), modelResourceLocation));
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(proxyClass, "net.minecraft.client.renderer.block.model.ModelResourceLocation",
                                           "net.minecraft.item.Item",
                                           "net.minecraftforge.client.model.ModelLoader");
        }

        private CodeCompileUnit CreateServerProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ServerProxy(Modname, Organization).ClassName, SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName);
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(proxyClass, "net.minecraft.item.Item");
        }

        private CodeCompileUnit CreateICommonProxyCodeUnit()
        {
            CodeMemberMethod registerItemMethod = CreateRegisterItemRendererMethod();
            registerItemMethod.CustomAttributes.Clear();
            return NewCodeUnit(NewInterface(SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName, registerItemMethod), "net.minecraft.item.Item");
        }

        private CodeMemberMethod CreateRegisterItemRendererMethod()
        {
            CodeMemberMethod method = NewMethod("registerItemRenderer", typeof(void).FullName, MemberAttributes.Public, new Parameter("Item", "item"),
                                                                                                new Parameter(typeof(int).FullName, "meta"),
                                                                                                new Parameter(typeof(string).FullName, "id"));
            method.CustomAttributes.Add(NewOverrideAnnotation());
            return method;
        }
    }
}
