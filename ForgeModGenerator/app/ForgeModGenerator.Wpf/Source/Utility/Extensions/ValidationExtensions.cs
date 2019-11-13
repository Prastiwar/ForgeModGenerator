using FluentValidation;
using ForgeModGenerator.Validation;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WhenCurrent<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, bool> predicate)
            => rule.When(predicate, ApplyConditionTo.CurrentValidator);

        public static IRuleBuilderOptions<T, TProperty> UniqueIn<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, IEnumerable<T> repository)
            => ruleBuilder.SetValidator(new UniquePropertyValidator<T, TProperty>(repository));

        public static IRuleBuilderOptions<T, TProperty> NotEmptyCollection<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
            => rule.Must(x => x is Array array ? array.Length > 0 : false);

        public static IRuleBuilderOptions<T, TProperty> NotEmptyCollection<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder.Must(x => x is Array array ? array.Length > 0 : false);
    }
}
