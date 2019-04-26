using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Validation
{
    public class SoundEventValidator : AbstractValidator<SoundEvent>, IUniqueValidator<SoundEvent>
    {
        public IEnumerable<SoundEvent> SoundEventRepository { get; private set; }

        public SoundEventValidator(IEnumerable<SoundEvent> soundEventRepository)
        {
            SetDefaultRepository(soundEventRepository);
            RuleFor(x => x.EventName).NotEmpty().WithMessage("Event Name cannot be empty")
                                     .Must(IsUnique).WithMessage("This name already exists");
        }
        public void SetDefaultRepository(IEnumerable<SoundEvent> instances) => SoundEventRepository = instances;

        private bool IsUnique(string name) => SoundEventRepository.HasOnlyOne(x => x.EventName == name);

        ValidateResult ForgeModGenerator.Validation.IValidator<SoundEvent>.Validate(SoundEvent instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult ForgeModGenerator.Validation.IValidator<SoundEvent>.Validate(SoundEvent instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));

        ValidateResult IUniqueValidator<SoundEvent>.Validate(SoundEvent instance, IEnumerable<SoundEvent> instances)
        {
            IEnumerable<SoundEvent> oldRepository = SoundEventRepository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(Validate(instance));
            SetDefaultRepository(oldRepository);
            return results;
        }

        ValidateResult IUniqueValidator<SoundEvent>.Validate(SoundEvent instance, IEnumerable<SoundEvent> instances, string propertyName)
        {
            IEnumerable<SoundEvent> oldRepository = SoundEventRepository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
            SetDefaultRepository(oldRepository);
            return results;
        }
    }
}
