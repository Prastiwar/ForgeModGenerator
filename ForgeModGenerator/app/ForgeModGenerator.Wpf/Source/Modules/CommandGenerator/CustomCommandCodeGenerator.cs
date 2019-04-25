using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CodeGeneration.CodeDom;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.CommandGenerator.CodeGeneration
{
    public class CustomCommandCodeGenerator : CustomScriptGenerator<Command>
    {
        public CustomCommandCodeGenerator(McMod mcMod, Command command) : base(mcMod, command) => ScriptLocator = SourceCodeLocator.CustomCommand(mcMod.ModInfo.Name, mcMod.Organization, command.Name);

        public override ClassLocator ScriptLocator { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeCompileUnit unit = base.CreateTargetCodeUnit();

            CodeMemberMethod getName = NewMethod("getName", typeof(string).FullName, MemberAttributes.Public);
            getName.Statements.Add(NewReturnPrimitive(Element.Name));

            CodeMemberMethod getUsage = NewMethod("getUsage", typeof(string).FullName, MemberAttributes.Public, new Parameter("ICommandSender", "sender"));
            getUsage.Statements.Add(NewReturnPrimitive(Element.Usage));

            JavaCodeMemberMethod execute = NewMethod("execute", typeof(string).FullName, MemberAttributes.Public, new Parameter("MinecraftServer", "server"),
                                                                                                              new Parameter("ICommandSender", "sender"),
                                                                                                              new Parameter("String[]", "args"));
            execute.ThrowsExceptions.Add("CommandException");

            CodeMemberMethod checkPermission = NewMethod("checkPermission", typeof(bool).FullName, MemberAttributes.Public, new Parameter("MinecraftServer", "server"),
                                                                                                                            new Parameter("ICommandSender", "sender"));
            checkPermission.Statements.Add(NewReturnPrimitive(false));

            CodeMemberMethod getRequiredPermissionLevel = NewMethod("getRequiredPermissionLevel", typeof(int).FullName, MemberAttributes.Public);
            getUsage.Statements.Add(NewReturnPrimitive(Element.PermissionLevel));

            unit.Namespaces[0].Types[0].Members.Add(getName);
            unit.Namespaces[0].Types[0].Members.Add(getUsage);
            unit.Namespaces[0].Types[0].Members.Add(execute);
            unit.Namespaces[0].Types[0].Members.Add(checkPermission);
            unit.Namespaces[0].Types[0].Members.Add(getRequiredPermissionLevel);

            unit.Namespaces[0].Imports.Add(NewImport("net.minecraft.command.CommandBase"));
            unit.Namespaces[0].Imports.Add(NewImport("net.minecraft.command.CommandException"));
            unit.Namespaces[0].Imports.Add(NewImport("net.minecraft.server.MinecraftServer"));
            unit.Namespaces[0].Imports.Add(NewImport("net.minecraft.command.ICommandSender"));

            return unit;
        }
    }
}
