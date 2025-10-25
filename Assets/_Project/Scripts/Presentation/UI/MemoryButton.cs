using _Project.Scripts.Application;
using _Project.Scripts.Presentation.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.UI
{
    public class MemoryButton : MonoBehaviour
    {
        [SerializeField] private string memorySceneName;

        void Awake()
        {
            MemorySystem memorySystem = ServiceLocater.GetService<MemorySystem>();
            
            GetComponent<Button>().onClick.AddListener(() =>
            {
                memorySystem.ReVisitMemory(memorySceneName);
            });
        }
    }
}