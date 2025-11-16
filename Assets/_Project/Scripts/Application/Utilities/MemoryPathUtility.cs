using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Application.Utilities
{
    public static class MemoryPathUtility
    {
        public static List<Vector3> ResolvePathPositions(List<string> names)
        {
            var result = new List<Vector3>();

            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var obj = GameObject.Find(name);
                if (obj != null)
                {
                    result.Add(obj.transform.position);
                }
                else
                {
                    Debug.LogWarning($"MemoryPathUtility: No GameObject found with name '{name}'.");
                }
            }

            return result;
        }
    }
}