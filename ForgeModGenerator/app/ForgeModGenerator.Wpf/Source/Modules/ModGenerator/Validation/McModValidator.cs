using FluentValidation;
using ForgeModGenerator.Models;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.ModGenerator.Validation
{
    public class McModValidator : AbstractUniqueValidator<McMod>
    {
        protected string LowerMatch => "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected string NameMatch => "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public McModValidator(IEnumerable<McMod> instances) : base(instances)
        {
            RuleFor(x => x.Organization).Matches(LowerMatch).WithMessage("Organization is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Modid).Must(IsUnique).WithMessage("This name already exists")
                                 .Matches(LowerMatch).WithMessage("Modid is not valid, should be lowercased, no numbers, length limit is 3-21");
            RuleFor(x => x.Name).Must(IsUnique).WithMessage("This name already exists")
                                .Matches(NameMatch).WithMessage("Name is not valid, first letter must be upper case, no numbers, length limit is 3-21");
        }
    }
}
