using ForgeModGenerator.Validations;
using System.Windows.Controls;

namespace ForgeModGenerator
{
    public interface IValidable
    {
        ValidationResult IsValid { get; }
    }

    public interface IValidable<T>
    {
        event ValidationEventHandler<T> Validate;
    }
}
