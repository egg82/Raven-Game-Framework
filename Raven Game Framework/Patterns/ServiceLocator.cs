using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using JoshuaKearney.Collections;
using Raven.Overrides;

namespace Raven.Patterns {
    public class ServiceLocator {
        // vars
        private static ConcurrentSet<Type> services = new ConcurrentSet<Type>();
        private static ConcurrentDictionary<Type, object> initializedServices = new ConcurrentDictionary<Type, object>();
        private static ConcurrentDictionary<Type, object> lookupCache = new ConcurrentDictionary<Type, object>();

        // constructor
        private ServiceLocator() {

        }

        // public
        public static T GetService<T>() {
            Type type = typeof(T);

            if (!initializedServices.TryGetValue(type, out object result) && services.Contains(type)) {
                result = initializedServices.AddIfAbsent(type, InitializeService(type));
            }

            if (result == null) {
                lookupCache.TryGetValue(type, out result);
            }

            if (result == null) {
                foreach (Type t in services) {
                    if (type.Equals(t) || type.IsAssignableFrom(t)) {
                        if (!initializedServices.TryGetValue(t, out result)) {
                            result = initializedServices.AddIfAbsent(type, InitializeService(t));
                        }
                        lookupCache.TryAdd(type, result);
                        break;
                    }
                }
            }

            return (result != null) ? (T) result : default(T);
        }
        public static void ProvideService(Type type, bool lazyInitialize = true) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            // Destroy existing services & cache
            initializedServices.TryRemove(type, out _);
            foreach (Type t in lookupCache.Keys) {
                if (t.Equals(type) || t.IsAssignableFrom(type)) {
                    lookupCache.TryRemove(t, out _);
                }
            }

            if (!lazyInitialize) {
                initializedServices.TryAdd(type, InitializeService(type));
            }

            services.Add(type);
        }
        public static void ProvideService(object initializedService) {
            if (initializedService == null) {
                throw new ArgumentNullException("initializedService");
            }

            Type type = initializedService.GetType();

            // Destroy existing services & cache
            initializedServices.TryRemove(type, out _);
            foreach (Type t in lookupCache.Keys) {
                if (t.Equals(type) || t.IsAssignableFrom(type)) {
                    lookupCache.TryRemove(t, out _);
                }
            }
            initializedServices.TryAdd(type, initializedService);

            services.Add(type);
        }

        public static ImmutableList<T> RemoveServices<T>() {
            Type type = typeof(T);

            List<T> retVal = new List<T>();

            foreach (Type t in lookupCache.Keys) {
                if (t.Equals(type) || t.IsAssignableFrom(type)) {
                    lookupCache.TryRemove(t, out _);
                }
            }

            foreach (Type t in services) {
                if (type.Equals(t) || type.IsAssignableFrom(t)) {
                    services.Remove(t);
                    if (initializedServices.TryRemove(t, out object result)) {
                        retVal.Add((T) result);
                    }
                }
            }

            return ImmutableList.ToImmutableList(retVal);
        }

        public static bool HasService<T>() {
            Type type = typeof(T);

            bool result = services.Contains(type);

            if (!result) {
                foreach (Type t in services) {
                    if (type.Equals(t) || type.IsAssignableFrom(t)) {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        public static bool ServiceIsInitialized<T>() {
            Type type = typeof(T);

            bool result = initializedServices.ContainsKey(type);

            if (!result) {
                foreach (Type t in services) {
                    if (type.Equals(t) || type.IsAssignableFrom(t)) {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        // private
        private static object InitializeService(Type type) {
            return Activator.CreateInstance(type);
        }
    }
}