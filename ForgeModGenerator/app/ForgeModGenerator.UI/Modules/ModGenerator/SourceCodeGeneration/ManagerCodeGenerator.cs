using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ManagerCodeGenerator : ScriptCodeGenerator
    {
        public ManagerCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = ModPaths.GeneratedModManagerFile(Modname, Organization);

        protected override string ScriptFilePath { get; }

        private CodeMemberMethod CretePreInitMethod()
        {
            CodeMemberMethod preInitMethod = CreateEmptyEventHandler("preInit", "FMLPreInitializationEvent");
            CodeVariableReferenceExpression loggerReference = new CodeVariableReferenceExpression("logger");
            CodeMethodInvokeExpression getModLog = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("event"), "getModLog");
            CodeAssignStatement assignLogger = new CodeAssignStatement(loggerReference, getModLog);
            preInitMethod.Statements.Add(assignLogger);
            //CodeMethodInvokeExpression registerWorldGenerator =
            //    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("GameRegistry"), "registerWorldGenerator", new CodeObjectCreateExpression($"{Modname}WorldGen"), new CodePrimitiveExpression(3));
            //preInitMethod.Statements.Add(registerWorldGenerator);
            return preInitMethod;
        }

        private CodeMemberMethod CreateInitMethod()
        {
            // TODO: Add annotation @EventHandler
            CodeMemberMethod initMethod = new CodeMemberMethod() {
                Name = "init",
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("void"),
            };
            CodeMethodInvokeExpression initRecipes = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression($"{Modname}Recipes"), "init");
            initMethod.Statements.Add(initRecipes);
            initMethod.Parameters.Add(new CodeParameterDeclarationExpression("FMLInitializationEvent", "event"));
            return initMethod;
        }

        private CodeMemberMethod CreateGetProxyMethod()
        {
            // TODO: Add annotation @EventHandler
            CodeMemberMethod getProxyMethod = new CodeMemberMethod() {
                Name = "getProxy",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                ReturnType = new CodeTypeReference("ICommonProxy"),
            };
            CodeMethodReturnStatement returnProxy = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("proxy"));
            getProxyMethod.Statements.Add(returnProxy);
            return getProxyMethod;
        }

        private CodeMemberMethod CreateEmptyEventHandler(string name, string eventType)
        {
            // TODO: Add annotation @EventHandler
            CodeMemberMethod method = new CodeMemberMethod() {
                Name = name,
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference("void"),
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(eventType, "event"));
            return method;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration managerClass = GetDefaultClass(null, true);

            // TODO: Add annotation @Instance
            CodeMemberField instanceField = new CodeMemberField(Modname, "instance") {
                Attributes = MemberAttributes.Private | MemberAttributes.Static
            };
            managerClass.Members.Add(instanceField);

            // TODO: Add annotation @SidedProxy(clientSide = {modname}Hook.CLIENTPROXYCLASS, serverSide = {modname}Hook.SERVERPROXYCLASS)
            CodeMemberField proxyField = new CodeMemberField("CommonProxy", "proxy") {
                Attributes = MemberAttributes.Private | MemberAttributes.Static
            };
            managerClass.Members.Add(proxyField);

            CodeMemberField loggerField = new CodeMemberField("Logger", "logger") {
                Attributes = MemberAttributes.Private | MemberAttributes.Static
            };
            managerClass.Members.Add(loggerField);

            managerClass.Members.Add(CretePreInitMethod());
            managerClass.Members.Add(CreateInitMethod());
            managerClass.Members.Add(CreateEmptyEventHandler("postInit", "FMLPostInitializationEvent"));
            managerClass.Members.Add(CreateEmptyEventHandler("serverStart", "FMLServerStartingEvent"));
            managerClass.Members.Add(CreateGetProxyMethod());

            CodeNamespace package = GetDefaultPackage(managerClass,
                                                      "net.minecraftforge.fml.common.Mod",
                                                      "net.minecraftforge.fml.common.SidedProxy",
                                                      "net.minecraftforge.fml.common.Mod.EventHandler",
                                                      "net.minecraftforge.fml.common.Mod.Instance",
                                                      "net.minecraftforge.fml.common.event.FMLInitializationEvent",
                                                      "net.minecraftforge.fml.common.event.FMLPostInitializationEvent",
                                                      "net.minecraftforge.fml.common.event.FMLPreInitializationEvent",
                                                      "net.minecraftforge.fml.common.event.FMLServerStartingEvent",
                                                      $"com.{Organization}.{Modname}.{Modname}Recipes",
                                                      $"com.{Organization}.{Modname}.proxy.CommonProxy",
                                                      "org.apache.logging.log4j.Logger"
            );
            return GetDefaultCodeUnit(package);
        }
    }
}
