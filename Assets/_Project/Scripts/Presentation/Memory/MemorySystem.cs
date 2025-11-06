using System;

using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemorySystem : MonoBehaviour
    {
        [SerializeField] private MemoryTransitionView transitionView;

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
        }
        
        private void HandleMemoryTransitionStart()
        {
            transitionView.Play();
        }

        private void HandleMemoryTransitionEnd()
        {
            Debug.Log("Memory transition end");
        }

        private void OnDestroy()
        {
            MemoryEvents.OnMemoryTransitionStart -= HandleMemoryTransitionStart;
            MemoryEvents.OnMemoryTransitionEnd -= HandleMemoryTransitionEnd;
        }
    }
}