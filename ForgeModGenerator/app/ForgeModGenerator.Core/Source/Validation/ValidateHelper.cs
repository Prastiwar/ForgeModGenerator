using System;

namespace ForgeModGenerator.Validation
{
    public static class ValidateHelper
    {
        /// <summary> Full lowercase letters, length limit 2-20 </summary>
        public static string LowerNameMatch => "^[a-z]{2,20}$";

        /// <summary> Error message for LowerNameMatch </summary>
        public static string LowerNameMatchError => "Name must be lowercase letters, length limit is 2-20";

        /// <summary> First upper letter, next letters case dont matter, length limit 2-20 </summary>
        public static string NameMatch => "^[A-Z]+[A-z]{2,20}$";

        /// <summary> Error message for LowerNameMatch </summary>
        public static string NameMatchError => "First character must be upper letter, length limit is 2-20";

        /// <summary> Returns first error occured on ValidationEventHandler or null if there is no error </summary>
        public static string OnValidateError(PropertyValidationEventHandler validate, object sender, string propertyName)
        {
            if (validate != null)
            {
                foreach (Delegate handler in validate.GetInvocationList())
                {
                    string error = ((PropertyValidationEventHandler)handler).Invoke(sender, propertyName);
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
