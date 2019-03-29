using ForgeModGenerator.Utility;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ForgeModGenerator.Controls
{
    public class PathNameSubmitTextBox : SubmitTextBox
    {
        public PathNameSubmitTextBox() : base() => TextSubmitted += UpdateFullPath;

        public static readonly DependencyProperty FullPathProperty =
            DependencyProperty.Register("FullPath", typeof(string), typeof(PathNameSubmitTextBox), new PropertyMetadata(null));
        public string FullPath {
            get => (string)GetValue(FullPathProperty);
            set => SetValue(FullPathProperty, value);
        }

        protected override bool CanSubmitText(SubmitTextBox sender, string text)
        {
            if (!(sender is PathNameSubmitTextBox pathSender))
            {
                return false;
            }
            if (!IOHelper.IsPathValid(pathSender.FullPath))
            {
                return false;
            }
            BindingExpression binding = pathSender.GetBindingExpression(FullPathProperty);
            if (binding == null)
            {
                return false;
            }

            FileSystemInfo info = IOHelper.GetFileSystemInfo(FullPath);
            string oldFullPath = pathSender.FullPath;
            string newFullPath = null;
            if (info is FileInfo fileInfo)
            {
                newFullPath = Path.Combine(fileInfo.DirectoryName, text);
            }
            else if (info is DirectoryInfo dirInfo)
            {
                newFullPath = Path.Combine(dirInfo.Parent.FullName, text);
            }
            else
            {
                return false;
            }
            string sourcePath = binding.GetResolvedValue<string>();
            if (!IOExtensions.ComparePath(sourcePath, newFullPath))
            {
                foreach (ValidationRule rule in binding.ParentBinding.ValidationRules)
                {
                    ValidationResult result = rule.Validate(newFullPath, null);
                    if (!result.IsValid)
                    {
                        ValidationError error = new ValidationError(rule, binding, result.ErrorContent, null);
                        System.Windows.Controls.Validation.MarkInvalid(binding, error);
                        return false;
                    }
                }
            }
            System.Windows.Controls.Validation.ClearInvalid(binding);
            return true;
        }

        private void UpdateFullPath(object sender, string e)
        {
            if (sender is PathNameSubmitTextBox pathSender)
            {
                BindingExpression binding = pathSender.GetBindingExpression(FullPathProperty);
                if (binding != null)
                {
                    binding.UpdateTarget();
                }
            }
        }
    }
}
