using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalMemoryManager : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetMemoryData(string memoryId)
        {
            nameText.text = memoryId;
        }

        public void LoadMemory()
        {
            
        }
    }
}
