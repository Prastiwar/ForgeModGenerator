using System;
using System.Runtime.Serialization;

namespace ForgeModGenerator
{
    /// <summary> Thrown when static class was not properly initialized </summary>
    [Serializable]
    public class ClassNotInitializedException : Exception
    {
        public ClassNotInitializedException() { }
        public ClassNotInitializedException(string message) : base(message) { }
        public ClassNotInitializedException(string message, Exception innerException) : base(message, innerException) { }

        protected ClassNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
