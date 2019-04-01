namespace ForgeModGenerator
{
    public static class StaticMessage
    {
        public const string UnauthorizedAccessMessage = "You do not have privilaged to perform this action.";

        public static string GetOperationFailedMessage(string name) => $"Operation failed. Check if {name} is not used by any process and you have privilages to do this.";
    }
}
