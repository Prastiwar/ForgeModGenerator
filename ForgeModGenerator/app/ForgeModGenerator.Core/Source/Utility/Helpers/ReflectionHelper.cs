﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForgeModGenerator.Utility
{
    public static class ReflectionHelper
    {
        private const BindingFlags NonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags PublicFlags = BindingFlags.Public | BindingFlags.Instance;

        public static T ParseEnum<T>(string value) => (T)Enum.Parse(typeof(T), value, true);
        public static IEnumerable<T> GetEnumValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static T CreateInstance<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);
        public static T CreateInstance<T>(params object[] args) => (T)Activator.CreateInstance(typeof(T), args);
        public static T CreateInstance<T>(bool nonPublic, params object[] args) => (T)Activator.CreateInstance(typeof(T), nonPublic ? NonPublicFlags : PublicFlags, null, args, null);

        public static IEnumerable<Type> GetSubclassTypes<T>() where T : class => GetSubclassTypes(typeof(T));

        public static IEnumerable<Type> GetSubclassTypes(Type baseType) => Assembly.GetAssembly(baseType).GetSubclassTypes(baseType);

        /// <summary> Enumerates over lazily created instance of sub classes of <typeparamref name="T"/> </summary>
        public static IEnumerable<T> EnumerateSubclasses<T>(bool throwOnCtorFail, params object[] constructorArgs) where T : class
        {
            if (throwOnCtorFail)
            {
                foreach (Type type in GetSubclassTypes<T>())
                {
                    yield return (T)Activator.CreateInstance(type, constructorArgs);
                }
            }
            else
            {
                foreach (Type type in GetSubclassTypes<T>())
                {
                    T instance = null;
                    try
                    {
                        instance = (T)Activator.CreateInstance(type, constructorArgs);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex: ex, moreInformation: "Forced to ignore this exception");
                        continue;
                    }
                    yield return instance;
                }
            }
        }
    }
}
