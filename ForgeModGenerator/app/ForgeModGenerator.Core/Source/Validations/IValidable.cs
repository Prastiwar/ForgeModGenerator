namespace ForgeModGenerator.Validation
{
    public delegate string PropertyValidationEventHandler<T>(T sender, string propertyName);

    public class ValidateResult
    {
        public ValidateResult(bool isValid, string error)
        {
            IsValid = isValid;
            Error = error;
        }
        static ValidateResult() => Valid = new ValidateResult(true, null);

        public static ValidateResult Valid { get; }

        public bool IsValid { get; }
        public string Error { get; }
    }

    public interface IValidable
    {
        ValidateResult Validate();
    }

    public interface IValidable<T> : IValidable
    {
        event PropertyValidationEventHandler<T> ValidateProperty;
    }
}
