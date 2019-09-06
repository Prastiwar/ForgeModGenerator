using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.Collections.Generic;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ProxyCodeGenerator : MultiScriptsCodeGenerator
    {
        public ProxyCodeGenerator(McMod mcMod) : base(mcMod) =>
            ScriptGenerators = new Dictionary<ClassLocator, GenerateDelegateHandler> {
                { SourceCodeLocator.CommonProxyInterface(Modname, Organization), CreateICommonProxyCodeUnit },
                { SourceCodeLocator.ClientProxy(Modname, Organization), CreateClientProxyCodeUnit },
                { SourceCodeLocator.ServerProxy(Modname, Organization), CreateServerProxyCodeUnit }
            };

        public override Dictionary<ClassLocator, GenerateDelegateHandler> ScriptGenerators { get; }


        private CodeCompileUnit CreateClientProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ClientProxy(Modname, Organization).ClassName, SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName);
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            CodeObjectCreateExpression modelResourceLocation = NewObject("ModelResourceLocation", NewMethodInvokeVar("item", "getRegistryName"), NewVarReference("id"));
            registerItemRendererMethod.Statements.Add(NewMethodInvokeType("ModelLoader", "setCustomModelResourceLocation", NewVarReference("item"), NewVarReference("meta"), modelResourceLocation));
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(SourceCodeLocator.ClientProxy(Modname, Organization).PackageName, proxyClass,
                                           "net.minecraft.client.renderer.block.model.ModelResourceLocation",
                                           "net.minecraft.item.Item",
                                           "net.minecraftforge.client.model.ModelLoader");
        }

        private CodeCompileUnit CreateServerProxyCodeUnit()
        {
            CodeTypeDeclaration proxyClass = NewClassWithBases(SourceCodeLocator.ServerProxy(Modname, Organization).ClassName, SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName);
            CodeMemberMethod registerItemRendererMethod = CreateRegisterItemRendererMethod();
            registerItemRendererMethod.Attributes |= MemberAttributes.Override;
            proxyClass.Members.Add(registerItemRendererMethod);
            return NewCodeUnit(SourceCodeLocator.ServerProxy(Modname, Organization).PackageName, proxyClass, "net.minecraft.item.Item");
        }

        private CodeCompileUnit CreateICommonProxyCodeUnit()
        {
            CodeMemberMethod registerItemMethod = CreateRegisterItemRendererMethod();
            registerItemMethod.CustomAttributes.Clear();
            return NewCodeUnit(SourceCodeLocator.CommonProxyInterface(Modname, Organization).PackageName,
                                NewInterface(SourceCodeLocator.CommonProxyInterface(Modname, Organization).ClassName, registerItemMethod), "net.minecraft.item.Item");
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
