using System;
using System.Runtime.Serialization;

namespace ForgeModGenerator.Exceptions
{
    public class InvalidSynchronizationArgument : ArgumentException
    {
        protected static string GetDefaultMessage(string rootSyncronizationPath, string actualPath) => $"You cannot synchronize folder outside root path: {rootSyncronizationPath}. The actual folder path: {actualPath}";

        public InvalidSynchronizationArgument(string rootSyncronizationPath, string actualPath) : base(GetDefaultMessage(rootSyncronizationPath, actualPath)) { }

        public InvalidSynchronizationArgument(string message) : base(message) { }

        public InvalidSynchronizationArgument(string message, Exception innerException) : base(message, innerException) { }

        public InvalidSynchronizationArgument(string rootSyncronizationPath, string actualPath, Exception innerException) : base(GetDefaultMessage(rootSyncronizationPath, actualPath), null, innerException) { }

        protected InvalidSynchronizationArgument(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
