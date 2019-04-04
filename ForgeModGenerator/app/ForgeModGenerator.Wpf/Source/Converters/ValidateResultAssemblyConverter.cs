using FluentValidation.Results;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.Converters
{
    public class ValidateResultAssemblyConverter
    {
        public static ValidateResult Convert(ValidationResult result)
        {
            string error = result.Errors.Count > 0 ? result.Errors[0].ErrorMessage : "";
            return new ValidateResult(result.IsValid, error);
        }

        public static ValidationResult Convert(ValidateResult result) => result.IsValid
                                                                      ? new ValidationResult()
                                                                      : new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("", result.Error) });
    }
}
