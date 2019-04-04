using FluentValidation;
using ForgeModGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.SoundGenerator.Validations
{
    public class SoundEventValidator : AbstractValidator<SoundEvent>, Validation.IValidator<SoundEvent>
    {
        public IEnumerable<SoundEvent> SoundEventRepository { get; }

        public SoundEventValidator(IEnumerable<SoundEvent> soundEventRepository)
        {
            SoundEventRepository = soundEventRepository;
            RuleFor(x => x.EventName).NotEmpty().WithMessage("Event Name cannot be empty")
                                     .Must(IsUnique).WithMessage("This name already exists");
        }

        private bool IsUnique(string name) => SoundEventRepository.HasOnlyOne(x => x.EventName == name);

        ValidateResult Validation.IValidator<SoundEvent>.Validate(SoundEvent instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult Validation.IValidator<SoundEvent>.Validate(SoundEvent instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
    }
}
