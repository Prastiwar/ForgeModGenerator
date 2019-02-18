using System.Globalization;
using System.Windows.Controls;

namespace ForgeModGenerator.Validations
{
    public class NotEmptyRule : ValidationRule
    {
        public NotEmptyRule() : base() { }
        public NotEmptyRule(ValidationStep validationStep, bool validatesOnTargetUpdated) : base(validationStep, validatesOnTargetUpdated) { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return value is string text
                ? new ValidationResult(!string.IsNullOrEmpty(text), "Value cannot be empty")
                : new ValidationResult(value != null, "Value cannot be null");
        }
    }
}
