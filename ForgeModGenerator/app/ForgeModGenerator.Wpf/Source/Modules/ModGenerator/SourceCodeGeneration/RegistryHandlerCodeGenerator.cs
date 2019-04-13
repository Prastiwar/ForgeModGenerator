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

        public override string ScriptFilePath { get; }

        private CodeMemberMethod GetRegisterMethod(string className, string fieldName, string registerType)
        {
            CodeMemberMethod method = NewMethod($"on{registerType}Register", typeof(void).FullName, MemberAttributes.Public | JavaAttributes.StaticOnly,
                                                                                                     new Parameter($"RegistryEvent.Register<{registerType}>", "event"));
            method.CustomAttributes.Add(NewSubscribeEventAnnotation());
            CodeMethodInvokeExpression getRegistry = NewMethodInvokeVar("event", "getRegistry");
            CodeFieldReferenceExpression list = NewFieldReferenceVar(className, fieldName);
            CodeMethodInvokeExpression registerParam = new CodeMethodInvokeExpression(list, "toArray", NewArray(registerType, 0));
            CodeMethodInvokeExpression registerAll = new CodeMethodInvokeExpression(getRegistry, "registerAll", registerParam);
            method.Statements.Add(registerAll);
            return method;
        }

        private CodeForeachStatement CreateRegisterModelForeach(string className, string registerType)
        {
            CodeForeachStatement loop = new CodeForeachStatement(NewVariable(registerType, registerType.ToLower()), NewFieldReferenceType(className, $"{registerType.ToUpper()}S"));
            CodeMethodInvokeExpression registerModels = NewMethodInvokeVar($"(({SourceCodeLocator.ModelInterface.ClassName}) {registerType.ToLower()})", "registerModels");
            CodeConditionStatement ifStatement = new CodeConditionStatement(
                new CodeSnippetExpression($"{registerType.ToLower()} instanceof {SourceCodeLocator.ModelInterface.ClassName}"), new CodeExpressionStatement(registerModels)
            );
            loop.Statements.Add(ifStatement);
            return loop;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(SourceCodeLocator.RegistryHandler.ClassName, GetRegisterMethod(SourceCodeLocator.Items.ClassName, SourceCodeLocator.Items.InitFieldName, "Item"),
                                                                               GetRegisterMethod(SourceCodeLocator.Blocks.ClassName, SourceCodeLocator.Blocks.InitFieldName, "Block"),
                                                                               GetRegisterMethod(SourceCodeLocator.SoundEvents.ClassName, SourceCodeLocator.SoundEvents.InitFieldName, "SoundEvent"));
            clas.CustomAttributes.Add(NewEventBusSubscriberAnnotation());
            CodeMemberMethod modelRegister = NewMethod("onModelRegister", typeof(void).FullName, MemberAttributes.Public | JavaAttributes.StaticOnly, new Parameter("ModelRegistryEvent", "event"));
            modelRegister.CustomAttributes.Add(NewSubscribeEventAnnotation());
            modelRegister.Statements.Add(CreateRegisterModelForeach(SourceCodeLocator.Items.ClassName, "Item"));
            modelRegister.Statements.Add(CreateRegisterModelForeach(SourceCodeLocator.Blocks.ClassName, "Block"));
            clas.Members.Add(modelRegister);

            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Blocks.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.Items.ImportFullName}",
                                     $"{PackageName}.{SourceCodeLocator.SoundEvents.ImportFullName}",
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
