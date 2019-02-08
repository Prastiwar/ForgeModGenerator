using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.ValidationRules
{
    public class SoundValidationDependencyWrapper : DependencyObject
    {
        public static readonly DependencyProperty SoundBeforeChangeProperty =
            DependencyProperty.Register("SoundBeforeChange", typeof(Sound), typeof(SoundValidationDependencyWrapper), new PropertyMetadata(null));
        public Sound SoundBeforeChange {
            get { return (Sound)GetValue(SoundBeforeChangeProperty); }
            set { SetValue(SoundBeforeChangeProperty, value); }
        }

        public static readonly DependencyProperty SoundsProperty =
            DependencyProperty.Register("Sounds", typeof(IEnumerable<Sound>), typeof(SoundValidationDependencyWrapper), new PropertyMetadata(null));
        public IEnumerable<Sound> Sounds {
            get { return (IEnumerable<Sound>)GetValue(SoundsProperty); }
            set { SetValue(SoundsProperty, value); }
        }
    }

    public class SoundRules : ValidationRule
    {
        public SoundRules() : base() { }
        public SoundRules(ValidationStep validationStep, bool validatesOnTargetUpdated) : base(validationStep, validatesOnTargetUpdated) { }
        public SoundRules(string propertyName, SoundValidationDependencyWrapper parameters)
        {
            PropertyName = propertyName;
            Parameters = parameters;
        }

        public string PropertyName { get; set; }
        public SoundValidationDependencyWrapper Parameters { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (PropertyName)
            {
                case nameof(Sound.Name):
                    return ValidateShortPath(value.ToString(), Parameters);
                case nameof(Sound.Type):
                    return ValidateType(value.ToString());
                default:
                    throw new NotImplementedException($"Validation of paremeter: {PropertyName} is not implemented");
            }
        }

        public ValidationResult ValidateShortPath(string newShortPath, SoundValidationDependencyWrapper parameters)
        {            
            ValidationResult emptyResult = new NotEmptyRule().Validate(newShortPath, null);
            if (!emptyResult.IsValid)
            {
                return emptyResult;
            }
            if (parameters == null || parameters.Sounds == null || parameters.SoundBeforeChange == null)
            {
                return new ValidationResult(false, "Validation failed, one of parameters was null");
            }
            string modname = Mod.GetModnameFromPath(parameters.SoundBeforeChange.Info.FullName);
            string modid = Mod.GetModidFromPath(parameters.SoundBeforeChange.Name);
            string soundsFolderPath = ModPaths.SoundsFolder(modname, modid);
            string oldFilePath = parameters.SoundBeforeChange.Info.FullName.Replace("\\", "/");
            string extension = Path.GetExtension(oldFilePath);
            string newFilePath = null;
            try
            {
                string newFilePathToValidate = $"{Path.Combine(soundsFolderPath, newShortPath)}{extension}";
                newFilePath = Path.GetFullPath(newFilePathToValidate).Replace("\\", "/");
            }
            catch (Exception)
            {
                return new ValidationResult(false, $"Path {newShortPath} is not valid for");
            }
            if (!IOExtensions.IsSubPathOf(newFilePath, soundsFolderPath))
            {
                return new ValidationResult(false, $"Path must be in {soundsFolderPath}");
            }
            if (parameters.SoundBeforeChange.ShortPath != newShortPath)
            {
                int nameCount = parameters.Sounds.Count(x => x.ShortPath == newShortPath) + 1;
                ValidationResult existResult = new ValidationResult(nameCount <= 1, $"{newShortPath} already exists");
                if (!existResult.IsValid)
                {
                    return existResult;
                }
            }
            return ValidationResult.ValidResult;
        }

        public ValidationResult ValidateType(string type) => new ValidationResult(Enum.GetValues(typeof(Sound.SoundType)).Cast<Sound.SoundType>().Any(x => x.ToString() == type), $"Type {type} is not supported");
    }
}
