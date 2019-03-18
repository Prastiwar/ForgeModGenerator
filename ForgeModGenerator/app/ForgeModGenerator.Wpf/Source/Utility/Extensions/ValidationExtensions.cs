using FluentValidation;
using System;

namespace ForgeModGenerator.Utility
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WhenCurrent<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, bool> predicate)
            => rule.When(predicate, ApplyConditionTo.CurrentValidator);
    }
}
