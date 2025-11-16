using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Application.MemoryPointer
{
    public static class MemoryImportantRegistry
    {
        private static readonly Dictionary<string, Transform> Points = new();

        public static void RegisterPoint(string id, Transform t)
        {
            Points[id] = t;
        }

        public static Transform GetPoint(string id)
        {
            return Points.GetValueOrDefault(id);
        }

        public static void Clear()
        {
            Points.Clear();
        }
    }
}