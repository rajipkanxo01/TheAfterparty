using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Presentation.MemoryPointer;
using UnityEngine;

namespace _Project.Scripts.Application.MemoryPointer
{
    public class MemoryPointerManager : MonoBehaviour
    {
        [SerializeField] private OffscreenPointer pointer;

        private void OnEnable()
        {
            FragmentEvents.OnSetActiveFragmentPoint += HandlePointChanged;
        }

        private void OnDisable()
        {
            FragmentEvents.OnSetActiveFragmentPoint -= HandlePointChanged;
        }

        private void HandlePointChanged(Transform t)
        {
            pointer.SetTarget(t);
        }
    }
}