using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForgeModGenerator.Utility
{
    public static class ReflectionExtensions
    {
        private const BindingFlags NonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags PublicFlags = BindingFlags.Public | BindingFlags.Instance;

        public static T CreateInstance<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);
        public static T CreateInstance<T>(params object[] args) => (T)Activator.CreateInstance(typeof(T), args);
        public static T CreateInstance<T>(bool nonPublic, params object[] args) => (T)Activator.CreateInstance(typeof(T), NonPublicFlags, null, args, null);

        public static bool HasProperty(this Type obj, string propertyName) => obj.GetProperty(propertyName) != null;

        public static bool HasDefaultConstructor<T>() => typeof(T).HasDefaultConstructor();
        public static bool HasDefaultConstructor(this Type t) => t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;

        public static IEnumerable<Type> GetSubclassTypes<T>(this Assembly assembly) where T : class => assembly.GetSubclassTypes(typeof(T));
        public static IEnumerable<Type> GetSubclassTypes(this Assembly assembly, Type baseType) => assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType));
    }
}
