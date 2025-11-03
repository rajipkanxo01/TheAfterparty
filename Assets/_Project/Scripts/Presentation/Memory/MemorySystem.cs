using System.Collections;
using _Project.Scripts.Application;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemorySystem : MonoBehaviour
    {
        // [SerializeField] private MemoryTransitionView transitionView;

        private MemoryManager _memoryManager;

        private void Start()
        {
            _memoryManager = ServiceLocater.GetService<MemoryManager>();
            
            // subscribe to application events
            // _memoryManager.MemoryTransitionStarted += OnTransitionStarted;
            _memoryManager.MemoryTransitionCompleted += OnTransitionCompleted;

            // application subscribe to presentation callback
            // transitionView.OnTransitionComplete += _memoryManager.OnVisualsDone;
            
            ServiceLocater.RegisterService(this);
            // DontDestroyOnLoad(this);
        }
        
        private void OnTransitionCompleted(string sceneName)
        {
            StartCoroutine(LoadSceneAfterDelay(sceneName, 2f));
        }

        private IEnumerator  LoadSceneAfterDelay(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }
        
        private void OnTransitionStarted()
        {
            Debug.Log("OnTransitionStarted");
            // transitionView.Play();
        }

        public void ReVisitMemory(string memorySceneName)
        {
            _memoryManager.RevisitMemory(memorySceneName);
        }
    }
}