using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Application
{
    public static class ServiceLocater
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        
        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} is already registered. Overwriting.");
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
            }
        }
        
        public static T GetService<T>()
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service of type {typeof(T).Name} not found.");
            return default;
        }
        
        public static void Clear()
        {
            services.Clear();
        }

    }
}