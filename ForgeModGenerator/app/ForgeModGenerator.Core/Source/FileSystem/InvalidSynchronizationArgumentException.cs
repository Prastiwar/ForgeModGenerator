using System;
using System.Runtime.Serialization;

namespace ForgeModGenerator.Exceptions
{
    [Serializable]
    public class InvalidSynchronizationArgumentException : ArgumentException
    {
        protected static string GetDefaultMessage(string rootSyncronizationPath, string actualPath) => $"You cannot synchronize folder outside root path: {rootSyncronizationPath}. The actual folder path: {actualPath}";

        public InvalidSynchronizationArgumentException(string rootSyncronizationPath, string actualPath) : base(GetDefaultMessage(rootSyncronizationPath, actualPath)) { }

        public InvalidSynchronizationArgumentException(string message) : base(message) { }

        public InvalidSynchronizationArgumentException(string message, Exception innerException) : base(message, innerException) { }

        public InvalidSynchronizationArgumentException(string rootSyncronizationPath, string actualPath, Exception innerException) : base(GetDefaultMessage(rootSyncronizationPath, actualPath), null, innerException) { }

        protected InvalidSynchronizationArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
