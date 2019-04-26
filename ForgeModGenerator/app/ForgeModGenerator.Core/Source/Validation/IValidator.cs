using System.Collections.Generic;

namespace ForgeModGenerator.Validation
{
    public interface IValidator<T>
    {
        ValidateResult Validate(T instance);
        ValidateResult Validate(T instance, string propertyName);
    }

    public interface IUniqueValidator<T> : IValidator<T>
    {
        void SetDefaultRepository(IEnumerable<T> instances);

        ValidateResult Validate(T instance, IEnumerable<T> instances);
        ValidateResult Validate(T instance, IEnumerable<T> instances, string propertyName);
    }
}
