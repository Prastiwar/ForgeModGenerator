using FluentValidation;
using ForgeModGenerator.Utility;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace ForgeModGenerator.Validation
{
    public class FullPathValidator : AbstractValidator<string>
    {
        public string RootPath { get; set; }

        /// <summary> Validates fullpath if is valid or can exists </summary>
        /// <param name="rootPath"> If is not null, validate as path must be subpath of rootPath </param>
        public FullPathValidator(string rootPath = null)
        {
            RootPath = rootPath;
            RuleFor(x => x).Must(ValidPath).WithMessage("Path is not valid").WhenCurrent(x => string.IsNullOrEmpty(RootPath))
                           .Must(path => ValidPath(Path.Combine(RootPath, path))).WithMessage("Path is not valid").WhenCurrent(x => !string.IsNullOrEmpty(RootPath))
                           .Must(NotExist).WithMessage("Path already exists").WhenCurrent(x => string.IsNullOrEmpty(RootPath))
                           .Must(path => NotExist(Path.Combine(RootPath, path))).WithMessage("Path already exists").WhenCurrent(x => !string.IsNullOrEmpty(RootPath))
                           .Must(path => HasSlash(path)).WithMessage("Cannot change root folder").WhenCurrent(x => !string.IsNullOrEmpty(RootPath))
                           .Must(HasFileName).WhenCurrent(x => IOHelper.IsFilePath(x));
        }

        private bool HasFileName(string fullPath) => new FileInfo(fullPath).Name != null;

        private bool ValidPath(string fullPath) => IOHelper.IsPathValid(fullPath);

        private bool NotExist(string fullPath) => !IOHelper.PathExists(fullPath);

        private bool HasSlash(string pathName) => pathName.NormalizePath().Contains("/");
    }

    public class FullPathValidatorRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            FullPathValidator validator = new FullPathValidator();
            FluentValidation.Results.ValidationResult results = validator.Validate(value.ToString());
            return new ValidationResult(results.IsValid, results.Errors.Count > 0 ? results.Errors[0] : null);
        }
    }
}
