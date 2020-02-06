using ForgeModGenerator.Models;

namespace ForgeModGenerator.CommandGenerator.Models
{
    public sealed class Command : ObservableModel
    {
        private string className;
        public string ClassName {
            get => className;
            set => SetProperty(ref className, value);
        }

        private string usage;
        public string Usage {
            get => usage;
            set => SetProperty(ref usage, value);
        }

        private string permission;
        public string Permission {
            get => permission;
            set => SetProperty(ref permission, value);
        }

        private int permissionLevel;
        public int PermissionLevel {
            get => permissionLevel;
            set => SetProperty(ref permissionLevel, value);
        }

        public override object DeepClone()
        {
            Command command = new Command() {
                ClassName = ClassName,
                Name = Name,
                Usage = Usage,
                Permission = Permission,
                PermissionLevel = PermissionLevel
            };
            command.IsDirty = false;
            return command;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Command command)
            {
                ClassName = command.ClassName;
                Name = command.Name;
                Usage = command.Usage;
                Permission = command.Permission;
                PermissionLevel = command.PermissionLevel;
                SetValidateProperty(command);
                IsDirty = false;
                return true;
            }
            return false;
        }
    }
}
