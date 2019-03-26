using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using ForgeModGenerator.Validations;
using Prism.Mvvm;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator
{
    public interface IFileNameChangeService
    {
        FileSystemInfoReference InfoReference { get; set; }
    }

    public class FileNameChanger : BindableBase, IDataErrorInfo, IFileNameChangeService
    {
        private FileSystemInfoReference infoReference;
        public FileSystemInfoReference InfoReference {
            get => infoReference;
            set {
               infoReference = value;
                if (infoReference != null)
                {
                    infoReference.PropertyChanged += OnInfoChanged;
                }
            }
        }

        public ValidateResult IsValid => new ValidateResult(error != null, "Path is not valid");

        private string error;
        string IDataErrorInfo.Error => error;
        string IDataErrorInfo.this[string propertyName] => ((IDataErrorInfo)this).Error;

        private string changeName;
        public string ChangeName {
            get => changeName;
            set {
                if (!SetProperty(ref changeName, value))
                {
                    return;
                }
                string newPath = null;
                if (InfoReference.Info is DirectoryInfo)
                {
                    newPath = new DirectoryInfo(InfoReference.FullName).Parent.FullName + "\\" + value;
                }
                else
                {
                    string valueWithExt = value + Path.GetExtension(InfoReference.Name);
                    newPath = new FileInfo(InfoReference.FullName).Directory.FullName + "\\" + valueWithExt;
                }
                if (InfoReference.FullName != newPath)
                {
                    FluentValidation.Results.ValidationResult validationResults = new FullPathValidator().Validate(newPath);
                    if (validationResults.IsValid)
                    {
                        if (IOHelper.IsFilePath(InfoReference.FullName))
                        {
                            IOHelperWin.MoveFile(InfoReference.FullName, newPath);
                        }
                        else
                        {
                            IOHelperWin.MoveDirectory(InfoReference.FullName, newPath);
                        }
                        InfoReference.SetInfo(newPath);
                        error = null;
                    }
                    else
                    {
                        error = validationResults.ToString();
                    }
                }
                else
                {
                    error = null;
                }
            }
        }

        private void OnInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            changeName = InfoReference.Info is DirectoryInfo ? InfoReference.Name : Path.GetFileNameWithoutExtension(InfoReference.Name);
            RaisePropertyChanged(nameof(ChangeName));
        }
    }
}
