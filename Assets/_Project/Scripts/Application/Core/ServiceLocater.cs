using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Application.Core
{
    public static class ServiceLocater
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        
        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);
            services[type] = service;
        }
        
        public static T GetService<T>()
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service of type {type.Name} not found. " +
                           $"Registered types: {string.Join(", ", services.Keys)}");
            return default;
        }

        
        public static void PrintAllServices()
        {
            Debug.Log("[ServiceLocator] Registered services:");
            foreach (var service in services)
            {
                Debug.Log($"- {service.Key.Name}");
            }
        }
        
        public static void Clear()
        {
            services.Clear();
        }

    }
}