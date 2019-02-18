using System;

namespace ForgeModGenerator
{
    public interface ICopiable : ICloneable
    {
        bool CopyValues(object fromCopy);
        object DeepClone();
    }
}
