using FluentValidation;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.RecipeGenerator.Validation
{
    public class RecipeValidator : AbstractUniqueValidator<Recipe>
    {
        public RecipeValidator(IEnumerable<Recipe> repository) : base(repository)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                .Must(IsUnique).WithMessage("This name already exists");
            RuleFor(x => ((ShapelessRecipe)x).Ingredients).NotEmpty().WithMessage("Ingredients cannot be empty");
        }
    }
}
