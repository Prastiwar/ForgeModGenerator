using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.ViewModels;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.CommandGenerator.ViewModels
{
    public class CommandGeneratorViewModel : FileInitViewModelBase<Command>
    {
        public CommandGeneratorViewModel(GeneratorContext<Command> context, ISynchronizeInvoke synchronizeInvoke) 
            : base(context, synchronizeInvoke) => FileSearchPatterns = "*.java";

        protected override string InitFilePath
            => SourceCodeLocator.Commands(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization).FullPath;

        public override string DirectoryRootPath => SessionContext.SelectedMod != null
            ? Path.GetDirectoryName(SourceCodeLocator.CustomCommand(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization, "None").FullPath)
            : null;

        protected override Command ParseModel(string content)
        {
            Command command = new Command();

            string classKeyword = " class ";
            int indexOfUsage = content.IndexOf(classKeyword);
            if (indexOfUsage > -1)
            {
                int startIndex = indexOfUsage + classKeyword.Length;
                int endIndex = content.IndexOf(' ', startIndex);
                command.ClassName = content.Substring(startIndex, endIndex - startIndex);
            }
            command.Name = InitializeProperty<string>(content, "public String getName()");
            command.Usage = InitializeProperty<string>(content, "public String getUsage(ICommandSender sender)");
            command.PermissionLevel = InitializeProperty<int>(content, "public int getRequiredPermissionLevel()");

            return command;
        }

        private T InitializeProperty<T>(string content, string findString)
        {
            int indexOfUsage = content.IndexOf(findString);
            if (indexOfUsage > -1)
            {
                string returnKeyword = "return ";
                indexOfUsage = content.IndexOf(returnKeyword, indexOfUsage + findString.Length);
                if (indexOfUsage > -1)
                {
                    int startIndex = indexOfUsage + returnKeyword.Length;
                    int endIndex = content.IndexOf(';', startIndex);
                    string value = content.Substring(startIndex, endIndex - startIndex);
                    if (value[0] == '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    return (T)System.Convert.ChangeType(value, typeof(T));
                }
            }
            return default;
        }
    }
}
