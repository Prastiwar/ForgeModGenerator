namespace ForgeModGenerator.Validation
{
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
}
