using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Validation;

namespace ForgeModGenerator.ModGenerator.Validation
{
    public class McModValidator : AbstractValidator<McMod>, ForgeModGenerator.Validation.IValidator<McMod>
    {
        protected string LowerMatch => "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected string NameMatch => "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public McModValidator()
        {
            RuleFor(x => x.Organization).Matches(LowerMatch).WithMessage("Organization is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Modid).Matches(LowerMatch).WithMessage("Modid is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Name).Matches(NameMatch).WithMessage("Name is not valid, first letter must be upper case, no numbers, length limit is 3-21");
        }

        ValidateResult ForgeModGenerator.Validation.IValidator<McMod>.Validate(McMod instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult ForgeModGenerator.Validation.IValidator<McMod>.Validate(McMod instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
    }
}
