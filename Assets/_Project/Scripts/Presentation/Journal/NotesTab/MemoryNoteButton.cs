using System;
using System.Linq;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class MemoryNoteButton : MonoBehaviour
    {
        [SerializeField] private string memoryID;
        [SerializeField] private Toggle toggle;

        [Header("Lock State")]
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private bool disableToggleInstead = true;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI notesText;
        
        private PlayerProfile _playerProfile;
        private bool _isUnlocked;
        
        public bool IsUnlocked => _isUnlocked;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    ShowNotesFor();
                }
            });
            
            UIEvents.OnJournalOpen += UpdateLockState;
        }

        private void Start()
        {
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("MemoryNoteButton: PlayerProfile not found.");
                return;
            }

            UpdateLockState();
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
            UIEvents.OnJournalOpen -= UpdateLockState;
        }

        public void UpdateLockState()
        {
            if (_playerProfile == null)
            {
                return;
            }

            bool unlocked = _playerProfile.HasUnlockedMemory(memoryID);
            _isUnlocked = unlocked;

            if (disableToggleInstead)
            {
                toggle.interactable = unlocked;
            }
            else if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!unlocked);
            }
        }

        public void ShowNotesFor()
        {
            if (_playerProfile == null)
                return;

            var memoryNotes = _playerProfile.GetAllMemoryNotes[memoryID];
            if (memoryNotes == null)
            {
                Debug.LogWarning($"MemoryNoteButton: No memory found for '{memoryID}'");
                ClearUI();
                return;
            }

            titleText.text = memoryID;

            notesText.text = memoryNotes.Count == 0
                ? "<i>No notes found yet...</i>"
                : "• " + string.Join("\n\n• ", memoryNotes);
        }
        
        private void ClearUI()
        {
            titleText.text = "";
            notesText.text = "";
        }
        
        public void SelectWithoutNotify()
        {
            toggle.SetIsOnWithoutNotify(true);
            ShowNotesFor();
        }
    }
}
