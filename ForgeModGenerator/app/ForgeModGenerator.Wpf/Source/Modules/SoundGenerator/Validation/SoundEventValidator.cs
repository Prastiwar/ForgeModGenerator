//using FluentValidation;
//using ForgeModGenerator.SoundGenerator.Models;
//using ForgeModGenerator.Validation;
//using System.Collections.Generic;

//namespace ForgeModGenerator.SoundGenerator.Validation
//{
//    public class SoundEventValidator : AbstractUniqueValidator<SoundEvent>
//    {
//        public SoundEventValidator(IEnumerable<SoundEvent> repository) : base(repository)
//            => RuleFor(x => x.EventName).NotEmpty().WithMessage("Event Name cannot be empty")
//                                        .Must(IsUnique).WithMessage("This Event Name already exists");
//    }
//}
