using _Project.Scripts.Application;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
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
            
            ServiceLocater.RegisterService(this);
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