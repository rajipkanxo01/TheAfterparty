using System;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemorySystem : MonoBehaviour
    {
        [SerializeField] private MemoryTransitionView transitionView;

        private MemoryManager memoryManager;

        private void Awake()
        {
            memoryManager = new MemoryManager();

            // subscribe to application events
            memoryManager.MemoryTransitionStarted += OnTransitionStarted;
            memoryManager.MemoryTransitionCompleted += OnTransitionCompleted;

            // application subscribe to presentation callback
            transitionView.OnTransitionComplete += memoryManager.OnVisualsDone;
            
            DontDestroyOnLoad(this);
        }

        private void OnTransitionCompleted(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void OnTransitionStarted()
        {
            transitionView.Play();
        }

        public void ReVisitMemory(string memorySceneName)
        {
            memoryManager.RevisitMemory(memorySceneName);
        }
    }
}