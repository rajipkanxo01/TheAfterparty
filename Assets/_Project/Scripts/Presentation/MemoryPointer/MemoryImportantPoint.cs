using _Project.Scripts.Application.MemoryPointer;
using UnityEngine;

namespace _Project.Scripts.Presentation.MemoryPointer
{
    public class MemoryImportantPoint : MonoBehaviour
    {
        [SerializeField] private string pointId;

        private void Awake()
        {
            MemoryImportantRegistry.RegisterPoint(pointId, transform);
        }
    }
}