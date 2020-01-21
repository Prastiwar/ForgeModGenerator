//using FluentValidation;
//using ForgeModGenerator.RecipeGenerator.Models;
//using ForgeModGenerator.Validation;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace ForgeModGenerator.RecipeGenerator.Validation
//{
//    public class RecipeValidator : AbstractUniqueValidator<Recipe>
//    {
//        public RecipeValidator(IEnumerable<Recipe> repository) : base(repository)
//        {
//            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
//                                .Must(IsUnique).WithMessage("This name already exists");

//            string emptyResultItemMsg = "Result Item cannot be empty";
//            string emptyIngredientsMsg = "Ingredients cannot be empty";
//            Func<Type, bool> hasResultProperty = type => type == typeof(ShapelessRecipe) || type == typeof(SmeltingRecipe) || type == typeof(ShapedRecipe);

//            RuleFor(x => ((RecipeCreator)x).Ingredients).Must(NotEmptyIngredients).WithMessage(emptyIngredientsMsg)
//                                                        .When(x => x is RecipeCreator creator && (creator.RecipeType == typeof(ShapelessRecipe) || creator.RecipeType == typeof(SmeltingRecipe)));
//            RuleFor(x => ((RecipeCreator)x).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
//                                                         .When(x => x is RecipeCreator creator && hasResultProperty(creator.RecipeType));

//            RuleFor(x => ((ShapelessRecipe)x).Ingredients).Must(NotEmptyIngredients).WithMessage(emptyIngredientsMsg)
//                                                          .When(x => x is ShapelessRecipe);
//            RuleFor(x => ((ShapelessRecipe)x).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
//                                                         .When(x => x is ShapelessRecipe);

//            RuleFor(x => ((SmeltingRecipe)x).Ingredients).Must(NotEmptyIngredients).WithMessage(emptyIngredientsMsg)
//                                                         .When(x => x is SmeltingRecipe);
//            RuleFor(x => ((SmeltingRecipe)x).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
//                                                         .When(x => x is SmeltingRecipe);

//            RuleFor(x => ((ShapedRecipe)x).Result.Item).NotEmpty().WithMessage(emptyResultItemMsg)
//                                                         .When(x => x is ShapedRecipe);
//        }

//        private bool NotEmptyIngredients(ObservableCollection<Ingredient> ingredients)
//        {
//            bool isEmpty = ingredients == null || ingredients.Count == 0;
//            if (!isEmpty)
//            {
//                foreach (Ingredient ingredient in ingredients)
//                {
//                    if (string.IsNullOrEmpty(ingredient.Item))
//                    {
//                        return false;
//                    }
//                }
//            }
//            return !isEmpty;
//        }
//    }
//}
