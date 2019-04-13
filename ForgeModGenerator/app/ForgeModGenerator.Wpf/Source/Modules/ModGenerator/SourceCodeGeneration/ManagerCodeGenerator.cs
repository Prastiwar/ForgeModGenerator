using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ManagerCodeGenerator : ScriptCodeGenerator
    {
        public ManagerCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.Manager.RelativePath);

        public override string ScriptFilePath { get; }

        private CodeMemberMethod CretePreInitMethod()
        {
            CodeMemberMethod method = CreateEmptyEventHandler("preInit", "FMLPreInitializationEvent");
            method.Statements.Add(new CodeAssignStatement(NewVarReference("logger"), NewMethodInvokeType("event", "getModLog")));
            //CodeMethodInvokeExpression registerWorldGenerator =
            //    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("GameRegistry"), "registerWorldGenerator", new CodeObjectCreateExpression($"{Modname}WorldGen"), new CodePrimitiveExpression(3));
            //preInitMethod.Statements.Add(registerWorldGenerator);
            return method;
        }

        private CodeMemberMethod CreateInitMethod()
        {
            CodeMemberMethod method = CreateEmptyEventHandler("init", "FMLInitializationEvent");
            method.Statements.Add(NewMethodInvokeType(SourceCodeLocator.Recipes.ClassName, "init"));
            return method;
        }

        private CodeMemberMethod CreateEmptyEventHandler(string name, string eventType)
        {
            CodeMemberMethod method = NewMethod(name, typeof(void).FullName, MemberAttributes.Public, new Parameter(eventType, "event"));
            method.CustomAttributes.Add(NewEventHandlerAnnotation());
            return method;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration managerClass = NewClassWithMembers(SourceCodeLocator.Manager.ClassName);
            string hook = SourceCodeLocator.Hook.ClassName;
            managerClass.CustomAttributes.Add(NewAnnotation("Mod",
                NewAnnotationArg("modid", NewFieldReferenceType(hook, "MODID")),
                NewAnnotationArg("name", NewFieldReferenceType(hook, "NAME")),
                NewAnnotationArg("version", NewFieldReferenceType(hook, "VERSION")),
                NewAnnotationArg("acceptedMinecraftVersions", NewFieldReferenceType(hook, "ACCEPTEDVERSIONS"))
            ));

            CodeMemberField instanceField = NewField(Modname, "instance", MemberAttributes.Private | JavaAttributes.StaticOnly);
            instanceField.CustomAttributes.Add(NewInstanceAnnotation());
            managerClass.Members.Add(instanceField);

            CodeMemberField proxyField = NewField("CommonProxy", "proxy", MemberAttributes.Private | JavaAttributes.StaticOnly);
            proxyField.CustomAttributes.Add(NewAnnotation("SidedProxy",
                NewAnnotationArg("clientSide", NewFieldReferenceType(hook, "CLIENTPROXYCLASS")),
                NewAnnotationArg("serverSide", NewFieldReferenceType(hook, "SERVERPROXYCLASS"))
            ));
            managerClass.Members.Add(proxyField);

            managerClass.Members.Add(NewField("Logger", "logger", MemberAttributes.Private | JavaAttributes.StaticOnly));

            managerClass.Members.Add(CretePreInitMethod());
            managerClass.Members.Add(CreateInitMethod());
            managerClass.Members.Add(CreateEmptyEventHandler("postInit", "FMLPostInitializationEvent"));
            managerClass.Members.Add(CreateEmptyEventHandler("serverStart", "FMLServerStartingEvent"));

            CodeMemberMethod getProxyMethod = NewMethod("getProxy", SourceCodeLocator.CommonProxyInterface.ClassName, MemberAttributes.Public | JavaAttributes.StaticOnly);
            getProxyMethod.Statements.Add(NewReturnVar("proxy"));
            managerClass.Members.Add(getProxyMethod);

            return NewCodeUnit(managerClass, "net.minecraftforge.fml.common.Mod",
                                             "net.minecraftforge.fml.common.SidedProxy",
                                             "net.minecraftforge.fml.common.Mod.EventHandler",
                                             "net.minecraftforge.fml.common.Mod.Instance",
                                             "net.minecraftforge.fml.common.event.FMLInitializationEvent",
                                             "net.minecraftforge.fml.common.event.FMLPostInitializationEvent",
                                             "net.minecraftforge.fml.common.event.FMLPreInitializationEvent",
                                             "net.minecraftforge.fml.common.event.FMLServerStartingEvent",
                                             $"{PackageName}.{SourceCodeLocator.Recipes.ImportFullName}",
                                             $"{PackageName}.{SourceCodeLocator.CommonProxyInterface.ImportFullName}",
                                             "org.apache.logging.log4j.Logger");
        }
    }
}
