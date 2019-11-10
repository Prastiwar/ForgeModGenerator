using FluentValidation;
using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Validation;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.BlockGenerator.Validation
{
    public class BlockValidator : AbstractValidator<Block>, IUniqueValidator<Block>
    {
        public IEnumerable<Block> Repository { get; private set; }

        public BlockValidator(IEnumerable<Block> BlockRepository)
        {
            SetDefaultRepository(BlockRepository);
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                .Must(IsUnique).WithMessage("This name already exists");
        }

        public void SetDefaultRepository(IEnumerable<Block> instances) => Repository = instances;

        private bool IsUnique(string name) => !Repository.Any(x => x.Name == name);

        ValidateResult ForgeModGenerator.Validation.IValidator<Block>.Validate(Block instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult ForgeModGenerator.Validation.IValidator<Block>.Validate(Block instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));

        ValidateResult IUniqueValidator<Block>.Validate(Block instance, IEnumerable<Block> instances)
        {
            Repository = instances;
            IEnumerable<Block> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(Validate(instance));
            SetDefaultRepository(oldRepository);
            return results;
        }

        ValidateResult IUniqueValidator<Block>.Validate(Block instance, IEnumerable<Block> instances, string propertyName)
        {
            IEnumerable<Block> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
            SetDefaultRepository(oldRepository);
            return results;
        }
    }
}
