//using ForgeModGenerator.CodeGeneration;
//using ForgeModGenerator.CommandGenerator.Models;
//using ForgeModGenerator.Models;
//using System.CodeDom;
//using System.Collections.Generic;
//using System.Linq;

//namespace ForgeModGenerator.CommandGenerator.CodeGeneration
//{
//    public class CommandCodeGenerator : InitVariablesCodeGenerator<Command>
//    {
//        public CommandCodeGenerator(McMod mcMod) : this(mcMod, Enumerable.Empty<Command>()) { }
//        public CommandCodeGenerator(McMod mcMod, IEnumerable<Command> elements) : base(mcMod, elements) => ScriptLocator = SourceCodeLocator.Commands(Modname, Organization);

//        public override ClassLocator ScriptLocator { get; }

//        protected override string GetElementName(Command element) => element.ClassName;

//        protected override CodeCompileUnit CreateTargetCodeUnit()
//        {
//            CodeTypeDeclaration clas = NewClassWithMembers(ScriptLocator.ClassName);
//            CodeMemberMethod registerAll = NewMethod("registerAll", typeof(void).FullName, MemberAttributes.Public | MemberAttributes.Static, new Parameter("FMLServerStartingEvent", "event"));
//            CodeNamespace package = NewPackage(ScriptLocator.PackageName, clas, "net.minecraftforge.fml.common.event.FMLServerStartingEvent");
//            foreach (Command element in Elements)
//            {
//                CodeMethodInvokeExpression register = NewMethodInvokeVar("event", "registerServerCommand", NewObject(GetElementName(element)));
//                registerAll.Statements.Add(register);
//                package.Imports.Add(NewImport(SourceCodeLocator.CustomCommand(Modname, Organization, element.ClassName).ImportFullName));
//            }
//            clas.Members.Add(registerAll);
//            return NewCodeUnit(package);
//        }
//    }
//}
