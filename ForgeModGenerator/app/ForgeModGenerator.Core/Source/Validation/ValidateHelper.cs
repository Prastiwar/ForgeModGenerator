using System;

namespace ForgeModGenerator.Validation
{
    public static class ValidateHelper
    {
        /// <summary> Returns first error occured on ValidationEventHandler or null if there is no error </summary>
        public static string OnValidateError<T>(PropertyValidationEventHandler<T> validate, T sender, string propertyName)
        {
            if (validate != null)
            {
                foreach (Delegate handler in validate.GetInvocationList())
                {
                    string error = ((PropertyValidationEventHandler<T>)handler).Invoke(sender, propertyName);
                    if (!string.IsNullOrEmpty(error))
                    {
                        return error;
                    }
                }
            }
            return null;
        }
    }
}
