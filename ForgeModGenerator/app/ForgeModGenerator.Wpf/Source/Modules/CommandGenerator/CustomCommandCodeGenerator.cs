using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Models;
using System.CodeDom;

namespace ForgeModGenerator.CommandGenerator.CodeGeneration
{
    public class CustomCommandCodeGenerator : ScriptCodeGenerator
    {
        public CustomCommandCodeGenerator(Mod mod, Command command) : base(mod)
        {
            ScriptLocator = SourceCodeLocator.CustomCommand(mod.ModInfo.Name, mod.Organization, command.Name);
            Command = command;
        }

        public override ClassLocator ScriptLocator { get; }

        protected Command Command { get; }

        protected override CodeCompileUnit CreateTargetCodeUnit()
        {
            CodeTypeDeclaration clas = NewClassWithMembers(ScriptLocator.ClassName);

            CodeMemberMethod getName = NewMethod("getName", typeof(string).FullName, MemberAttributes.Public);
            getName.Statements.Add(NewReturnPrimitive(Command.Name));

            CodeMemberMethod getUsage = NewMethod("getUsage", typeof(string).FullName, MemberAttributes.Public, new Parameter("ICommandSender", "sender"));
            getUsage.Statements.Add(NewReturnPrimitive(Command.Usage));

            CodeMemberMethod execute = NewMethod("execute", typeof(string).FullName, MemberAttributes.Public, new Parameter("MinecraftServer", "server"),
                                                                                                              new Parameter("ICommandSender", "sender"),
                                                                                                              new Parameter("String[]", "args")); // TODO: add throws CommandException

            CodeMemberMethod checkPermission = NewMethod("checkPermission", typeof(bool).FullName, MemberAttributes.Public, new Parameter("MinecraftServer", "server"),
                                                                                                                            new Parameter("ICommandSender", "sender"));
            checkPermission.Statements.Add(NewReturnPrimitive(false));

            CodeMemberMethod getRequiredPermissionLevel = NewMethod("getRequiredPermissionLevel", typeof(int).FullName, MemberAttributes.Public);
            getUsage.Statements.Add(NewReturnPrimitive(0));

            clas.Members.Add(getName);
            clas.Members.Add(getUsage);
            clas.Members.Add(execute);
            clas.Members.Add(checkPermission);
            clas.Members.Add(getRequiredPermissionLevel);

            CodeNamespace package = NewPackage(ScriptLocator.PackageName, clas,
                                                "net.minecraft.command.CommandBase",
                                                "net.minecraft.command.CommandException",
                                                "net.minecraft.server.MinecraftServer",
                                                "net.minecraft.command.ICommandSender");
            return NewCodeUnit(package);
        }
    }
}
