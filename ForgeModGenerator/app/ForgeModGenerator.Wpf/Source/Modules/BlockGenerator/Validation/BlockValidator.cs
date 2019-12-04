using FluentValidation;
using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.Validation;
using System.Collections.Generic;

namespace ForgeModGenerator.BlockGenerator.Validation
{
    public class BlockValidator : AbstractUniqueValidator<Block>
    {
        public BlockValidator(IEnumerable<Block> repository) : base(repository)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                                              .Must(IsUnique).WithMessage("This name already exists");
            RuleFor(x => x.MaterialType).NotEmpty().WithMessage("Block needs material type");
            RuleFor(x => x.InventoryTextureName).NotEmpty().WithMessage("Block needs the inventory texture");
            RuleFor(x => x.TextureName).NotEmpty().WithMessage("Block needs the texture");
        }
    }
}
