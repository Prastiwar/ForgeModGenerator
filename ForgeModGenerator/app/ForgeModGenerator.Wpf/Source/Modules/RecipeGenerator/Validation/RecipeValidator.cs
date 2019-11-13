using FluentValidation;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Validation
{
    public class RecipeValidator : AbstractUniqueValidator<Recipe>
    {
        public RecipeValidator(IEnumerable<Recipe> repository) : base(repository)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                .Must(IsUnique).WithMessage("This name already exists");
            RuleFor(x => ((ShapelessRecipe)x).Ingredients).Must(NotEmptyIngredients).WithMessage("Ingredients cannot be empty").When(x => x is ShapelessRecipe);
        }

        private bool NotEmptyIngredients(ObservableCollection<Ingredient> ingredients)
        {
            bool isEmpty = ingredients == null || ingredients.Count == 0;
            if (!isEmpty)
            {
                foreach (Ingredient ingredient in ingredients)
                {
                    if (string.IsNullOrEmpty(ingredient.Item))
                    {
                        return false;
                    }
                }
            }
            return !isEmpty;
        }
    }
}
