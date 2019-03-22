using System;
using System.Collections.Generic;
using System.Reflection;

namespace ForgeModGenerator.Utility
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetSubclassTypes<T>() where T : class => GetSubclassTypes(typeof(T));

        public static IEnumerable<Type> GetSubclassTypes(Type baseType) => Assembly.GetAssembly(baseType).GetSubclassTypes(baseType);

        public static IEnumerable<T> EnumerateSubclasses<T>(params object[] constructorArgs) where T : class
        {
            foreach (Type type in GetSubclassTypes<T>())
            {
                yield return (T)Activator.CreateInstance(type, constructorArgs);
            }
        }
    }
}
