//using FluentValidation;
//using ForgeModGenerator.CommandGenerator.Models;
//using ForgeModGenerator.Validation;
//using System.Collections.Generic;

//namespace ForgeModGenerator.CommandGenerator.Validation
//{
//    public class CommandValidator : AbstractUniqueValidator<Command>
//    {
//        public CommandValidator(IEnumerable<Command> repository) : base(repository)
//        {
//            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
//                                .Must(IsUnique).WithMessage("This name already exists");
//            RuleFor(x => x.ClassName).NotEmpty().WithMessage("ClassName cannot be empty")
//                                     .Must(IsUnique).WithMessage("This ClassName already exists");
//        }
//    }
//}
