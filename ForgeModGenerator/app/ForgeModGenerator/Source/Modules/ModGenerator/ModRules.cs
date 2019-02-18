using ForgeModGenerator.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ForgeModGenerator.ModGenerator.Validations
{
    public class ModRules : ValidationRule
    {
        public ModRules() : base() { }
        public ModRules(ValidationStep validationStep, bool validatesOnTargetUpdated) : base(validationStep, validatesOnTargetUpdated) { }
        public ModRules(string propertyName) => PropertyName = propertyName;

        protected readonly string lowerMatch = "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected readonly string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public string PropertyName { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (PropertyName)
            {
                case nameof(Mod.Organization):
                    return ValidateOrganization(value.ToString());
                case nameof(Mod.ModInfo.Modid):
                    return ValidateModid(value.ToString());
                case nameof(Mod.ModInfo.Name):
                    return ValidateName(value.ToString());
                default:
                    throw new NotImplementedException($"Validation of paremeter: {PropertyName} is not implemented");
            }
        }

        public ValidationResult ValidateOrganization(string organization) => new ValidationResult(Regex.IsMatch(organization, lowerMatch), "Organization is not valid, should be lowercased, no numbers, length limit is 3-21");
        public ValidationResult ValidateModid(string modid) => new ValidationResult(Regex.IsMatch(modid, lowerMatch), "Modid is not valid, should be lowercased, no numbers, length limit is 3-21");
        public ValidationResult ValidateName(string name) => new ValidationResult(Regex.IsMatch(name, nameMatch), "Name is not valid, first letter must be upper case, no numbers, length limit is 3-21");
    }
}
