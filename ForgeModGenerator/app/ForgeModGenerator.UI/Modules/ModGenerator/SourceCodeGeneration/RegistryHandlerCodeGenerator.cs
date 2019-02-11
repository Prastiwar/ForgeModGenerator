using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.ModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class RegistryHandlerCodeGenerator : ScriptCodeGenerator
    {
        public RegistryHandlerCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.GeneratedSourceCodeFolder(Modname, Organization), "handler", "RegistryHandler.java");

        protected override string ScriptFilePath { get; }

        private CodeMemberMethod GetRegisterMethod(string registerType)
        {
            // TODO: Add annotation @SubscribeEvent
            CodeMemberMethod method = new CodeMemberMethod() {
                Name = $"on{registerType}Register",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                ReturnType = new CodeTypeReference("void"),
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression($"RegistryEvent.Register<{registerType}>", "event"));
            CodeMethodInvokeExpression getRegistry = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("event"), "getRegistry");
            CodeFieldReferenceExpression list = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression($"{Modname}{registerType}s"), $"{registerType.ToUpper()}S");
            CodeMethodInvokeExpression registerParam = new CodeMethodInvokeExpression(list, "toArray", new CodeArrayCreateExpression($"{registerType}", 0));
            CodeMethodInvokeExpression registerAll = new CodeMethodInvokeExpression(getRegistry, "registerAll", registerParam);
            method.Statements.Add(registerAll);
            return method;
        }

        private CodeForeachStatement CreateRegisterModelForeach(string registerType)
        {
            CodeForeachStatement forLoop = new CodeForeachStatement(
                                                new CodeVariableDeclarationStatement(registerType, registerType.ToLower()),
                                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression($"{Modname}{registerType}s"), $"{registerType.ToUpper()}S"));
            CodeMethodInvokeExpression registerModels = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression($"((IHasModel) {registerType.ToLower()})"), "registerModels");
            CodeConditionStatement ifStatement = new CodeConditionStatement(new CodeSnippetExpression($"{registerType.ToLower()} instanceof IHasModel"), new CodeExpressionStatement(registerModels));
            forLoop.Statements.Add(ifStatement);
            return forLoop;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            // TODO: Add annotation @EventBusSubscriber
            CodeTypeDeclaration clas = GetDefaultClass("RegistryHandler", false);

            clas.Members.Add(GetRegisterMethod("Item"));
            clas.Members.Add(GetRegisterMethod("Block"));
            clas.Members.Add(GetRegisterMethod("SoundEvent"));

            // TODO: Add annotation @SubscribeEvent
            CodeMemberMethod modelRegister = new CodeMemberMethod() {
                Name = $"onModelRegister",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                ReturnType = new CodeTypeReference("void")
            };
            modelRegister.Parameters.Add(new CodeParameterDeclarationExpression($"ModelRegistryEvent", "event"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Item"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Block"));
            clas.Members.Add(modelRegister);

            CodeNamespace package = GetDefaultPackage(clas, $"{GeneratedPackageName}.{Modname}Blocks",
                                                            $"{GeneratedPackageName}.{Modname}Items",
                                                            $"{GeneratedPackageName}.{Modname}Sounds",
                                                            $"{GeneratedPackageName}.handler.IHasModel",
                                                            "net.minecraft.block.Block",
                                                            "net.minecraft.item.Item",
                                                            "net.minecraft.util.SoundEvent",
                                                            "net.minecraftforge.client.event.ModelRegistryEvent",
                                                            "net.minecraftforge.event.RegistryEvent",
                                                            "net.minecraftforge.fml.common.Mod.EventBusSubscriber",
                                                            "net.minecraftforge.fml.common.eventhandler.SubscribeEvent");
            return GetDefaultCodeUnit(package);
        }
    }
}
