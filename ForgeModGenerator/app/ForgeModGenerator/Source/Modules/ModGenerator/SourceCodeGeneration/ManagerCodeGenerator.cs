using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class ManagerCodeGenerator : ScriptCodeGenerator
    {
        public ManagerCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), Modname + ".java");

        protected override string ScriptFilePath { get; }

        private CodeMemberMethod CretePreInitMethod()
        {
            CodeMemberMethod preInitMethod = CreateEmptyEventHandler("preInit", "FMLPreInitializationEvent");
            preInitMethod.Statements.Add(new CodeAssignStatement(NewVarReference("logger"), NewMethodInvokeType("event", "getModLog")));
            //CodeMethodInvokeExpression registerWorldGenerator =
            //    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("GameRegistry"), "registerWorldGenerator", new CodeObjectCreateExpression($"{Modname}WorldGen"), new CodePrimitiveExpression(3));
            //preInitMethod.Statements.Add(registerWorldGenerator);
            return preInitMethod;
        }

        private CodeMemberMethod CreateInitMethod()
        {
            // TODO: Add annotation @EventHandler
            CodeMemberMethod initMethod = NewMethod("init", typeof(void).FullName, MemberAttributes.Public, new Parameter("FMLInitializationEvent", "event"));
            initMethod.Statements.Add(NewMethodInvokeType(Modname + "Recipes", "init"));
            return initMethod;
        }

        private CodeMemberMethod CreateEmptyEventHandler(string name, string eventType)
        {
            // TODO: Add annotation @EventHandler
            CodeMemberMethod method = NewMethod(name, typeof(void).FullName, MemberAttributes.Public, new Parameter(eventType, "event"));
            return method;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration managerClass = NewClassWithMembers(null, true);

            // TODO: Add annotation @Instance
            CodeMemberField instanceField = NewField(Modname, "instance", MemberAttributes.Private | MemberAttributes.Static);
            managerClass.Members.Add(instanceField);

            // TODO: Add annotation @SidedProxy(clientSide = {modname}Hook.CLIENTPROXYCLASS, serverSide = {modname}Hook.SERVERPROXYCLASS)
            CodeMemberField proxyField = NewField("CommonProxy", "proxy", MemberAttributes.Private | MemberAttributes.Static);
            managerClass.Members.Add(proxyField);

            managerClass.Members.Add(NewField("Logger", "logger", MemberAttributes.Private | MemberAttributes.Static));

            managerClass.Members.Add(CretePreInitMethod());
            managerClass.Members.Add(CreateInitMethod());
            managerClass.Members.Add(CreateEmptyEventHandler("postInit", "FMLPostInitializationEvent"));
            managerClass.Members.Add(CreateEmptyEventHandler("serverStart", "FMLServerStartingEvent"));

            // TODO: Add annotation @EventHandler
            CodeMemberMethod getProxyMethod = NewMethod("getProxy", "ICommonProxy", MemberAttributes.Public | MemberAttributes.Static);
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
                                             $"{GeneratedPackageName}.{Modname}Recipes",
                                             $"{GeneratedPackageName}.proxy.CommonProxy",
                                             "org.apache.logging.log4j.Logger");
        }
    }
}
