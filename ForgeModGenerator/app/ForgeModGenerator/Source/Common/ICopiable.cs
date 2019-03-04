using System;

namespace ForgeModGenerator
{
    public interface ICopiable : ICloneable
    {
        bool CopyValues(object fromCopy);
        object DeepClone();
    }

    public interface ICopiable<T> : ICopiable
    {
        bool CopyValues(T fromCopy);
        T DeepCopy();
        T ShallowCopy();
    }
}
