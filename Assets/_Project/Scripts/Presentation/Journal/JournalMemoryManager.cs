using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Presentation.Memory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalMemoryManager : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private string memorySceneName;

        public void SetMemoryData(string memoryId)
        {
            nameText.text = memoryId;
            memorySceneName = "Memory_" + memoryId;
        }

        public void LoadMemory()
        {
            if(memorySceneName != SceneManager.GetActiveScene().name)
            {
                // _memorySystem.ReVisitMemory(memorySceneName);
                MemoryEvents.RaiseVisitMemory(memorySceneName);
            }
        }
    }
}
