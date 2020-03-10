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

            string emptyResultItemMsg = "Result Item cannot be empty";
            string emptyIngredientsMsg = "Ingredients cannot be empty";

            RuleFor(x => (x as ShapelessRecipe).Ingredients).Must(NotEmptyIngredients).WithMessage(emptyIngredientsMsg)
                                                          .When(x => x is ShapelessRecipe);
            RuleFor(x => (x as ShapelessRecipe).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
                                                         .When(x => x is ShapelessRecipe);

            RuleFor(x => (x as SmeltingRecipe).Ingredients).Must(NotEmptyIngredients).WithMessage(emptyIngredientsMsg)
                                                         .When(x => x is SmeltingRecipe);
            RuleFor(x => (x as SmeltingRecipe).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
                                                         .When(x => x is SmeltingRecipe);

            RuleFor(x => (x as ShapedRecipe).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
                                                         .When(x => x is ShapedRecipe);
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
