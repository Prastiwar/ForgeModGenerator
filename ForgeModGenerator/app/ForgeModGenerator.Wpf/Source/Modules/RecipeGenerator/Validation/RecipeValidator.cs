using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Validation;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.RecipeGenerator.Validation
{
    public class RecipeValidator : AbstractValidator<Recipe>, IUniqueValidator<Recipe>
    {
        public IEnumerable<Recipe> Repository { get; private set; }

        public RecipeValidator(IEnumerable<Recipe> RecipeRepository)
        {
            SetDefaultRepository(RecipeRepository);
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                .Must(IsUnique).WithMessage("This name already exists");
            RuleFor(x => ((ShapelessRecipe)x).Ingredients).NotEmpty().WithMessage("Ingredients cannot be empty");
        }

        public void SetDefaultRepository(IEnumerable<Recipe> instances) => Repository = instances;

        private bool IsUnique(string name) => Repository.Where(x => x.Name == name).Take(2).Count() <= 0;

        ValidateResult ForgeModGenerator.Validation.IValidator<Recipe>.Validate(Recipe instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult ForgeModGenerator.Validation.IValidator<Recipe>.Validate(Recipe instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));

        ValidateResult IUniqueValidator<Recipe>.Validate(Recipe instance, IEnumerable<Recipe> instances)
        {
            Repository = instances;
            IEnumerable<Recipe> oldRepository = Repository; // save actual repository to roll it back later
            SetDefaultRepository(instances); // temporary change repository for validation
            ValidateResult results = ValidateResultAssemblyConverter.Convert(Validate(instance));
            SetDefaultRepository(oldRepository);
            return results;
        }

        ValidateResult IUniqueValidator<Recipe>.Validate(Recipe instance, IEnumerable<Recipe> instances, string propertyName)
        {
            IEnumerable<Recipe> oldRepository = Repository; // save actual repository to roll it back later
            SetDefaultRepository(instances); // temporary change repository for validation
            ValidateResult results = ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
            SetDefaultRepository(oldRepository);
            return results;
        }
    }
}
