using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Validation;

namespace ForgeModGenerator.ModGenerator.Validations
{
    public class ModValidator : AbstractValidator<Mod>, Validation.IValidator<Mod>
    {
        protected readonly string lowerMatch = "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected readonly string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public ModValidator()
        {
            RuleFor(x => x.Organization).Matches(lowerMatch).WithMessage("Organization is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Modid).Matches(lowerMatch).WithMessage("Modid is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Name).Matches(nameMatch).WithMessage("Name is not valid, first letter must be upper case, no numbers, length limit is 3-21");
        }

        ValidateResult Validation.IValidator<Mod>.Validate(Mod instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult Validation.IValidator<Mod>.Validate(Mod instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
    }
}
