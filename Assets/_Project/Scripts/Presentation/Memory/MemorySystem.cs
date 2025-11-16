using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemorySystem : MonoBehaviour
    {
        [SerializeField] private MemoryTransitionView transitionView;
        
        private MemoryFragmentService _fragmentService;

        private void Awake()
        {
            ServiceLocater.RegisterService(this);
        }

        private void Start()
        {
            if (transitionView == null)
            {
                transitionView = ServiceLocater.GetService<MemoryTransitionView>();
            }
            
            MemoryEvents.OnMemoryTransitionStart += HandleMemoryTransitionStart;
            MemoryEvents.OnMemoryTransitionEnd += HandleMemoryTransitionEnd;
            MemoryEvents.OnVisitMemory += HandleVisitMemory;
        }
        
        private void HandleMemoryTransitionStart()
        {
            transitionView.Play();
        }

        private void HandleMemoryTransitionEnd()
        {
            Debug.Log("Memory transition end");
        }
        
        private void HandleVisitMemory(string memoryId)
        {
            Debug.Log($"MemorySystem: Visiting memory '{memoryId}'");

            // Clean old service if exists
            if (_fragmentService != null)
            {
                Debug.Log("MemorySystem: Destroying previous MemoryFragmentService");
                _fragmentService.Dispose();
                _fragmentService = null;
            }

            // Create new service for this memory
            _fragmentService = new MemoryFragmentService(memoryId);
        }

        private void OnDestroy()
        {
            MemoryEvents.OnMemoryTransitionStart -= HandleMemoryTransitionStart;
            MemoryEvents.OnMemoryTransitionEnd -= HandleMemoryTransitionEnd;
        }
    }
}