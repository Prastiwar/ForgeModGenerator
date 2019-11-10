using FluentValidation;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Validation;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.CommandGenerator.Validation
{
    public class CommandValidator : AbstractValidator<Command>, IUniqueValidator<Command>
    {
        public IEnumerable<Command> Repository { get; private set; }

        public CommandValidator(IEnumerable<Command> commandRepository)
        {
            SetDefaultRepository(commandRepository);
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                .Must(IsUnique).WithMessage("This name already exists");
            RuleFor(x => x.ClassName).NotEmpty().WithMessage("ClassName cannot be empty")
                                     .Must(IsUnique).WithMessage("This ClassName already exists");
        }

        public void SetDefaultRepository(IEnumerable<Command> instances) => Repository = instances;

        private bool IsUnique(string name) => !Repository.Any(x => x.Name == name);

        ValidateResult ForgeModGenerator.Validation.IValidator<Command>.Validate(Command instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult ForgeModGenerator.Validation.IValidator<Command>.Validate(Command instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));

        ValidateResult IUniqueValidator<Command>.Validate(Command instance, IEnumerable<Command> instances)
        {
            Repository = instances;
            IEnumerable<Command> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(Validate(instance));
            SetDefaultRepository(oldRepository);
            return results;
        }

        ValidateResult IUniqueValidator<Command>.Validate(Command instance, IEnumerable<Command> instances, string propertyName)
        {
            IEnumerable<Command> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
            SetDefaultRepository(oldRepository);
            return results;
        }
    }
}
