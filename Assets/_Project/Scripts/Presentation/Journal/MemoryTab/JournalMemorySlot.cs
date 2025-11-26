using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.MemoryTab
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

        private MemoryDatabase _memoryDatabase;
        public string MemoryID => memoryId;

        private void OnEnable()
        {
            _memoryDatabase = ServiceLocater.GetService<MemoryDatabase>();
            if (_memoryDatabase == null)
            {
                Debug.LogError("JournalMemorySlot: MemoryDatabase service not found.");
                return;
            }
        }

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
            var memorySceneName = _memoryDatabase.GetById(memoryId).sceneName;

            portrait.sprite = unlockedSprite;
            nameText.text = unlockedName;
            
            if (SceneManager.GetActiveScene().name == memorySceneName)
            {
                // grey-out when current scene
                canvasGroup.alpha = 0.35f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return;
            }

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