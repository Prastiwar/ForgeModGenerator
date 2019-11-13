namespace ForgeModGenerator.Validation
{
    public delegate string PropertyValidationEventHandler(object sender, string propertyName);

    public interface IValidable
    {
        ValidateResult Validate();
        event PropertyValidationEventHandler ValidateProperty;
    }
}
