namespace ForgeModGenerator.Validation
{
    public interface IValidator<T>
    {
        ValidateResult Validate(T instance);
        ValidateResult Validate(T instance, string propertyName);
    }
}
