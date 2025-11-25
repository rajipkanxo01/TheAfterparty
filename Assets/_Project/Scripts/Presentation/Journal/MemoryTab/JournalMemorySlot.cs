using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Memory.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalMemorySlot : MonoBehaviour
    {
        [Header("Memory Info")]
        [SerializeField] private string memoryId;
        
        [Header("Unlocked State")]
        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private string unlockedName;

        [Header("Locked State")]
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private string lockedName = "???";

        [Header("UI")]
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public string MemoryID => memoryId;

        public void SetLocked()
        {
            portrait.sprite = lockedSprite;
            nameText.text = lockedName;

            // grey-out effect
            canvasGroup.alpha = 0.45f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void SetUnlocked()
        {
            portrait.sprite = unlockedSprite;
            nameText.text = unlockedName;

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void LoadMemory()
        {
            UIEvents.RaiseJournalClose();
            MemoryEvents.RaiseVisitMemory(memoryId);
        }
    }
}