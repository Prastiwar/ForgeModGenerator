namespace ForgeModGenerator.Validation
{
    public delegate string PropertyValidationEventHandler<T>(T sender, string propertyName);
    
    public interface IValidable
    {
        ValidateResult Validate();
    }

    public interface IValidable<T> : IValidable
    {
        event PropertyValidationEventHandler<T> ValidateProperty;
    }
}
