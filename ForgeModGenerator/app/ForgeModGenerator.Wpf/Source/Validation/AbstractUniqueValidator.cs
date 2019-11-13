using FluentValidation;
using FluentValidation.Validators;
using ForgeModGenerator.Converters;
using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.Validation
{
    public abstract class AbstractUniqueValidator<T> : AbstractValidator<T>, IUniqueValidator<T>
    {
        public IEnumerable<T> Repository { get; private set; }

        public AbstractUniqueValidator(IEnumerable<T> repository) => SetDefaultRepository(repository);

        protected virtual bool IsUnique(T model) => !Repository.Any(x => x.Equals(model));

        protected virtual bool IsUnique<TProperty>(T instance, TProperty propertyValue, PropertyValidatorContext context)
        {
            if (propertyValue == null)
            {
                return true;
            }
            bool isUnique = !Repository.Any(x => {
                if (!x.Equals(instance))
                {
                    object value = x.GetType().GetProperty(context.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(x);
                    if (value is TProperty val)
                    {
                        bool isEqual = val.Equals(propertyValue);
                        return isEqual;
                    }
                }
                return false;
            });
            return isUnique;
        }

        protected bool IsPropertyUnique<TValue>(T model, string propertyName, TValue value) => !Repository.Any(x => {
            object val = x.GetType().GetProperty(propertyName).GetValue(model);
            return val is TValue tVal && tVal.Equals(value);
        });

        public void SetDefaultRepository(IEnumerable<T> instances) => Repository = instances;

        ValidateResult IValidator<T>.Validate(T instance) => ValidateResultAssemblyConverter.Convert(Validate(instance));
        ValidateResult IValidator<T>.Validate(T instance, string propertyName) => ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));

        ValidateResult IUniqueValidator<T>.Validate(T instance, IEnumerable<T> instances)
        {
            Repository = instances;
            IEnumerable<T> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(Validate(instance));
            SetDefaultRepository(oldRepository);
            return results;
        }

        ValidateResult IUniqueValidator<T>.Validate(T instance, IEnumerable<T> instances, string propertyName)
        {
            IEnumerable<T> oldRepository = Repository;
            SetDefaultRepository(instances);
            ValidateResult results = ValidateResultAssemblyConverter.Convert(this.Validate(instance, propertyName));
            SetDefaultRepository(oldRepository);
            return results;
        }
    }
}
