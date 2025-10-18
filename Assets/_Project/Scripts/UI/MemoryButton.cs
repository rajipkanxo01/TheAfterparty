using _Project.Scripts.Presentation.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class MemoryButton : MonoBehaviour
    {
        [SerializeField] private string memorySceneName;
        [SerializeField] private MemorySystem memorySystem;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                memorySystem.ReVisitMemory(memorySceneName);
            });
        }
    }
}