using ForgeModGenerator.Utility;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Validations
{
    public class ValidPathRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) => value is string path
                                                                                         ? new ValidationResult(IOHelper.IsPathValid(path), "{path} is not valid path")
                                                                                         : new ValidationResult(false, "Validation failed, value is not string");
    }

    public class NotExistPathRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) => value is string path
                                                                                         ? new ValidationResult(!IOHelper.PathExists(path), "{path} already exists")
                                                                                         : new ValidationResult(false, "Validation failed, value is not string");
    }

    public class CanMoveFileSystemInfoDependencyWrapper : DependencyObject
    {
        public static readonly DependencyProperty MustBeInRootPathProperty =
            DependencyProperty.Register("MustBeInRootPath", typeof(bool), typeof(CanMoveFileSystemInfoDependencyWrapper), new PropertyMetadata(true));
        public bool MustBeInRootPath {
            get => (bool)GetValue(MustBeInRootPathProperty);
            set => SetValue(MustBeInRootPathProperty, value);
        }

        public static readonly DependencyProperty RootPathProperty =
            DependencyProperty.Register("RootPath", typeof(string), typeof(CanMoveFileSystemInfoDependencyWrapper), new PropertyMetadata(null));
        public string RootPath {
            get => (string)GetValue(RootPathProperty);
            set => SetValue(RootPathProperty, value);
        }
    }

    public class CanMoveFileSystemInfoRule : ValidationRule
    {
        public CanMoveFileSystemInfoDependencyWrapper Parameters { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string path = null;
            if (value is string stringPath)
            {
                if (!IOHelper.IsPathValid(stringPath))
                {
                    return new ValidationResult(false, "Invalid characters");
                }
                if (Parameters != null && !string.IsNullOrEmpty(Parameters.RootPath))
                {
                    string root = IOHelper.GetDirectoryPath(Parameters.RootPath);
                    if (Parameters.MustBeInRootPath && stringPath.Contains("/"))
                    {
                        return new ValidationResult(false, "Cannot change root folder");
                    }
                    path = Path.Combine(root, stringPath);
                }
                else
                {
                    path = stringPath;
                }
            }
            else if (value is FileSystemInfo fileSystemInfoPath)
            {
                path = fileSystemInfoPath.FullName;
            }
            else if (value is FileSystemInfoReference fileSystemInfoRefPath)
            {
                path = fileSystemInfoRefPath.FullName;
            }
            return ValidateFullPath(path);
        }

        public ValidationResult ValidateFullPath(string path)
        {
            ValidationResult validRuleResult = new ValidPathRule().Validate(path, null);
            if (!validRuleResult.IsValid)
            {
                return validRuleResult;
            }
            ValidationResult notExistRuleResult = new NotExistPathRule().Validate(path, null);
            if (!notExistRuleResult.IsValid)
            {
                return notExistRuleResult;
            }
            return ValidationResult.ValidResult;
        }
    }
}
