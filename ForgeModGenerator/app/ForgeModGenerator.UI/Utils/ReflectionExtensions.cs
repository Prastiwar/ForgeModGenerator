using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForgeModGenerator.Utils
{
    public static class ReflectionExtensions
    {
        public static bool HasDefaultConstructor<T>() => typeof(T).HasDefaultConstructor();
        public static bool HasDefaultConstructor(this Type t) => t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;

        public static IEnumerable<Type> GetSubclassTypes<T>() where T : class => GetSubclassTypes(typeof(T));
        public static IEnumerable<Type> GetSubclassTypes(Type baseType) => Assembly.GetAssembly(baseType).GetSubclassTypes(baseType);
        public static IEnumerable<Type> GetSubclassTypes<T>(this Assembly assembly) where T : class => assembly.GetSubclassTypes(typeof(T));
        public static IEnumerable<Type> GetSubclassTypes(this Assembly assembly, Type baseType) => assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType));

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
