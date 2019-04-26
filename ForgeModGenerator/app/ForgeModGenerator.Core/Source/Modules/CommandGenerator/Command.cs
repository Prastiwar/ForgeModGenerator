using ForgeModGenerator.Validation;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.CommandGenerator.Models
{
    public sealed class Command : FileObject, IDataErrorInfo, IValidable<Command>
    {
        private Command() { }

        public Command(string filePath) : base(filePath)
        {
            string content = File.ReadAllText(Info.FullName);
            InitializeProperty(ref usage, content, "public String getUsage(ICommandSender sender)");
            InitializeProperty(ref name, content, "public String getName()");
            InitializeProperty(ref permissionLevel, content, "public int getRequiredPermissionLevel()");
        }

        private void InitializeProperty<T>(ref T property, string content, string findString)
        {
            int indexOfUsage = content.IndexOf(findString);
            if (indexOfUsage > -1)
            {
                string returnKeyword = "return ";
                indexOfUsage = content.IndexOf("return ", indexOfUsage);
                if (indexOfUsage > -1)
                {
                    int startIndex = indexOfUsage + returnKeyword.Length;
                    int endIndex = content.IndexOf(';', indexOfUsage);
                    string value = content.Substring(startIndex, endIndex - startIndex);
                    if (value[0] == '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    property = (T)System.Convert.ChangeType(value, typeof(T));
                }
            }
        }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
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
                Name = Name,
                Usage = Usage,
                Permission = Permission,
                PermissionLevel = PermissionLevel
            };
            command.SetInfo(Info.FullName);
            command.IsDirty = false;
            return command;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Command command)
            {
                Name = command.Name;
                Usage = command.Usage;
                Permission = command.Permission;
                PermissionLevel = command.PermissionLevel;

                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }

        public ValidateResult Validate()
        {
            string errorString = OnValidate(nameof(Name));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(Usage));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(Permission));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(PermissionLevel));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            return ValidateResult.Valid;
        }

        public event PropertyValidationEventHandler<Command> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
