using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utils;
using ForgeModGenerator.Validations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Validations
{
    public class SoundValidationDependencyWrapper : DependencyObject
    {
        public static readonly DependencyProperty SoundBeforeChangeProperty =
            DependencyProperty.Register("SoundBeforeChange", typeof(Sound), typeof(SoundValidationDependencyWrapper), new PropertyMetadata(null));
        public Sound SoundBeforeChange {
            get => (Sound)GetValue(SoundBeforeChangeProperty);
            set => SetValue(SoundBeforeChangeProperty, value);
        }

        public static readonly DependencyProperty SoundsProperty =
            DependencyProperty.Register("Sounds", typeof(IEnumerable<Sound>), typeof(SoundValidationDependencyWrapper), new PropertyMetadata(null));
        public IEnumerable<Sound> Sounds {
            get => (IEnumerable<Sound>)GetValue(SoundsProperty);
            set => SetValue(SoundsProperty, value);
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
                    return ValidateName(value.ToString(), Parameters);
                case nameof(Sound.Type):
                    return ValidateType(value.ToString());
                default:
                    throw new NotImplementedException($"Validation of paremeter: {PropertyName} is not implemented");
            }
        }

        public ValidationResult ValidateName(string name, SoundValidationDependencyWrapper parameters)
        {
            ValidationResult emptyResult = new NotEmptyRule().Validate(name, null);
            if (!emptyResult.IsValid)
            {
                return emptyResult;
            }
            else if (parameters == null || parameters.Sounds == null || parameters.SoundBeforeChange == null)
            {
                return new ValidationResult(false, "Validation failed, one of parameters was null");
            }
            string relativePath = Sound.GetRelativePathFromSoundName(name);
            if (!IOExtensions.IsPathValid(relativePath))
            {
                return new ValidationResult(false, $"Filename {name} has invalid characters");
            }
            else if (parameters.SoundBeforeChange.Name != name)
            {
                int nameCount = parameters.Sounds.Count(x => x.Name == name) + 1;
                ValidationResult existResult = new ValidationResult(nameCount <= 1, $"{name} already exists");
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
