using System;

namespace ForgeModGenerator.Validations
{
    public delegate string ValidationEventHandler<T>(T sender, string propertyName);

    public static class ValidateHelper
    {
        /// <summary> Returns first error occured on ValidationEventHandler or null if there is no error </summary>
        public static string OnValidateError<T>(ValidationEventHandler<T> validate, T sender, string propertyName)
        {
            if (validate != null)
            {
                foreach (Delegate handler in validate.GetInvocationList())
                {
                    string error = ((ValidationEventHandler<T>)handler).Invoke(sender, propertyName);
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
