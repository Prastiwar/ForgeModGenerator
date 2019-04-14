using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.ModGenerator.SourceCodeGeneration
{
    public class RegistryHandlerCodeGenerator : ScriptCodeGenerator
    {
        public RegistryHandlerCodeGenerator(Mod mod) : base(mod) => ScriptLocator = SourceCodeLocator.RegistryHandler(Modname, Organization);

        public override ClassLocator ScriptLocator { get; }

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
            CodeMethodInvokeExpression registerModels = NewMethodInvokeVar($"(({SourceCodeLocator.ModelInterface(Modname, Organization).ClassName}) {registerType.ToLower()})", "registerModels");
            CodeConditionStatement ifStatement = new CodeConditionStatement(
                new CodeSnippetExpression($"{registerType.ToLower()} instanceof {SourceCodeLocator.ModelInterface(Modname, Organization).ClassName}"), new CodeExpressionStatement(registerModels)
            );
            loop.Statements.Add(ifStatement);
            return loop;
        }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(SourceCodeLocator.RegistryHandler(Modname, Organization).ClassName, GetRegisterMethod(SourceCodeLocator.Items(Modname, Organization).ClassName, SourceCodeLocator.Items(Modname, Organization).InitFieldName, "Item"),
                                                                               GetRegisterMethod(SourceCodeLocator.Blocks(Modname, Organization).ClassName, SourceCodeLocator.Blocks(Modname, Organization).InitFieldName, "Block"),
                                                                               GetRegisterMethod(SourceCodeLocator.SoundEvents(Modname, Organization).ClassName, SourceCodeLocator.SoundEvents(Modname, Organization).InitFieldName, "SoundEvent"));
            clas.CustomAttributes.Add(NewEventBusSubscriberAnnotation());
            CodeMemberMethod modelRegister = NewMethod("onModelRegister", typeof(void).FullName, MemberAttributes.Public | JavaAttributes.StaticOnly, new Parameter("ModelRegistryEvent", "event"));
            modelRegister.CustomAttributes.Add(NewSubscribeEventAnnotation());
            modelRegister.Statements.Add(CreateRegisterModelForeach(SourceCodeLocator.Items(Modname, Organization).ClassName, "Item"));
            modelRegister.Statements.Add(CreateRegisterModelForeach(SourceCodeLocator.Blocks(Modname, Organization).ClassName, "Block"));
            clas.Members.Add(modelRegister);

            return NewCodeUnit(clas, $"{PackageName}.{SourceCodeLocator.Blocks(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.Items(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.SoundEvents(Modname, Organization).ImportRelativeName}",
                                     $"{PackageName}.{SourceCodeLocator.ModelInterface(Modname, Organization).ImportRelativeName}",
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
