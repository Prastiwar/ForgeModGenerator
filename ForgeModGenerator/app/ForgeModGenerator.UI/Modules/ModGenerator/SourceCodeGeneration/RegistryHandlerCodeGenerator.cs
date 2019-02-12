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
            CodeMemberMethod method = NewMethod($"on{registerType}Register", typeof(void).FullName, MemberAttributes.Public | MemberAttributes.Static, new Parameter($"RegistryEvent.Register<{registerType}>", "event"));
            CodeMethodInvokeExpression getRegistry = NewMethodInvokeVar("event", "getRegistry");
            CodeFieldReferenceExpression list = NewFieldReferenceVar($"{Modname}{registerType}s", $"{registerType.ToUpper()}S");
            CodeMethodInvokeExpression registerParam = new CodeMethodInvokeExpression(list, "toArray", NewArray($"{registerType}", 0));
            CodeMethodInvokeExpression registerAll = new CodeMethodInvokeExpression(getRegistry, "registerAll", registerParam);
            method.Statements.Add(registerAll);
            return method;
        }

        private CodeForeachStatement CreateRegisterModelForeach(string registerType)
        {
            CodeForeachStatement loop = new CodeForeachStatement(NewVariable(registerType, registerType.ToLower()), NewFieldReferenceType($"{Modname}{registerType}s", $"{registerType.ToUpper()}S"));
            CodeMethodInvokeExpression registerModels = NewMethodInvokeVar($"((IHasModel) {registerType.ToLower()})", "registerModels");
            CodeConditionStatement ifStatement = new CodeConditionStatement(new CodeSnippetExpression($"{registerType.ToLower()} instanceof IHasModel"), new CodeExpressionStatement(registerModels));
            loop.Statements.Add(ifStatement);
            return loop;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            // TODO: Add annotation @EventBusSubscriber
            CodeTypeDeclaration clas = NewClassWithMembers("RegistryHandler", false, GetRegisterMethod("Item"),
                                                                                     GetRegisterMethod("Block"),
                                                                                     GetRegisterMethod("SoundEvent"));
            // TODO: Add annotation @SubscribeEvent
            CodeMemberMethod modelRegister = NewMethod("onModelRegister", typeof(void).FullName, MemberAttributes.Public | MemberAttributes.Static, new Parameter("ModelRegistryEvent", "event"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Item"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Block"));
            clas.Members.Add(modelRegister);

            return NewCodeUnit(clas, $"{GeneratedPackageName}.{Modname}Blocks",
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
        }
    }
}
