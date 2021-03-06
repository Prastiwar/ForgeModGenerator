﻿using System.IO;

namespace ForgeModGenerator.CommandGenerator.Models
{
    public sealed class Command : FileObject
    {
        private Command() { }

        public Command(string filePath) : base(filePath)
        {
            string content = File.ReadAllText(Info.FullName);
            int indexOfUsage = content.IndexOf(" class ");
            if (indexOfUsage > -1)
            {
                int startIndex = indexOfUsage + 7;
                int endIndex = content.IndexOf(' ', startIndex);
                ClassName = content.Substring(startIndex, endIndex - startIndex);
            }
            InitializeProperty(ref name, content, "public String getName()");
            InitializeProperty(ref usage, content, "public String getUsage(ICommandSender sender)");
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

        private string className;
        public string ClassName {
            get => className;
            set => SetProperty(ref className, value);
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
                ClassName = ClassName,
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
                ClassName = command.ClassName;
                Name = command.Name;
                Usage = command.Usage;
                Permission = command.Permission;
                PermissionLevel = command.PermissionLevel;
                SetValidateProperty(command);
                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }
    }
}
