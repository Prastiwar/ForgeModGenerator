using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;
using System.IO;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class RegistryHandlerCodeGenerator : ScriptCodeGenerator
    {
        public RegistryHandlerCodeGenerator(Mod mod) : base(mod) => ScriptFilePath = Path.Combine(ModPaths.SourceCodeRootFolder(Modname, Organization), SourceCodeLocator.RegistryHandler.RelativePath);

        protected override string ScriptFilePath { get; }

        private CodeMemberMethod GetRegisterMethod(string initClassName, string customRegisterType = null)
        {
            // TODO: Add annotation @SubscribeEvent
            string registerName = string.IsNullOrEmpty(customRegisterType) ? initClassName : customRegisterType;
            CodeMemberMethod method = NewMethod($"on{initClassName}Register", typeof(void).FullName, MemberAttributes.Public | JavaAttributes.StaticOnly,
                                                                                                     new Parameter($"RegistryEvent.Register<{registerName}>", "event"));
            CodeMethodInvokeExpression getRegistry = NewMethodInvokeVar("event", "getRegistry");
            CodeFieldReferenceExpression list = NewFieldReferenceVar($"{Modname}{initClassName}s", $"{initClassName.ToUpper()}S");
            CodeMethodInvokeExpression registerParam = new CodeMethodInvokeExpression(list, "toArray", NewArray(registerName, 0));
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
            CodeTypeDeclaration clas = NewClassWithMembers(SourceCodeLocator.RegistryHandler.ClassName, GetRegisterMethod("Item"),
                                                                                                        GetRegisterMethod("Block"),
                                                                                                        GetRegisterMethod("Sound", "SoundEvent"));
            // TODO: Add annotation @SubscribeEvent
            CodeMemberMethod modelRegister = NewMethod("onModelRegister", typeof(void).FullName, MemberAttributes.Public | JavaAttributes.StaticOnly, new Parameter("ModelRegistryEvent", "event"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Item"));
            modelRegister.Statements.Add(CreateRegisterModelForeach("Block"));
            clas.Members.Add(modelRegister);

            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Blocks.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Items.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Sounds.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface.ImportFullName}",
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
