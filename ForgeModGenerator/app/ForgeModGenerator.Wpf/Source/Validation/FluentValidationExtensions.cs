using FluentValidation;
using System.Collections.Generic;

namespace ForgeModGenerator.Validation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> UniqueIn<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, IEnumerable<T> repository)
            => ruleBuilder.SetValidator(new UniquePropertyValidator<T, TProperty>(repository));
    }
}
