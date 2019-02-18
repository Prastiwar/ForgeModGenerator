using System;
using System.Collections.Generic;
using System.Reflection;

namespace ForgeModGenerator.Utility
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetSubclassTypes<T>() where T : class => GetSubclassTypes(typeof(T));

        public static IEnumerable<Type> GetSubclassTypes(Type baseType) => Assembly.GetAssembly(baseType).GetSubclassTypes(baseType);

        public static IEnumerable<T> GetSubclasses<T>(params object[] constructorArgs) where T : class
        {
            List<T> subclasses = new List<T>(8);
            foreach (Type type in GetSubclassTypes<T>())
            {
                subclasses.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return subclasses;
        }
    }
}
