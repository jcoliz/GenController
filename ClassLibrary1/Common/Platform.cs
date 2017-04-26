using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Service Locator
    /// </summary>
    public static class Platform
    {
        private static Dictionary<Type, object> Objects;

        public static void Set<T>(T value) where T : class
        {
            if (Objects == null)
                Objects = new Dictionary<Type, object>();

            Objects[typeof(T)] = value;
        }
        public static T Get<T>() where T : class
        {
            if (Objects == null || !Objects.ContainsKey(typeof(T)))
            {
                throw new PlatformNotSupportedException($"Service {typeof(T).Name} not found.");
            }
            return Objects[typeof(T)] as T;
        }
        public static T TryGet<T>() where T : class
        {
            if (Objects == null || !Objects.ContainsKey(typeof(T)))
            {
                return null;
            }
            return Objects[typeof(T)] as T;
        }

        public static void Clear()
        {
            Objects.Clear();
        }
    }
}
