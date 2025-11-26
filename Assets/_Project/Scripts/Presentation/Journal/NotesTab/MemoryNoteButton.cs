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
        [SerializeField] private Transform observationContainer;
        [SerializeField] private GameObject observationPrefab;

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
                Debug.Log("MemoryNoteButton: Toggle is on, showing notes for " + memoryID);
                ShowNotesFor();
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

        private void ShowNotesFor()
        {
            foreach (Transform child in observationContainer)
            {
                Destroy(child.gameObject);
            }
            
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
            

            foreach (var observation in memoryNotes)
            {
                GameObject entry = Instantiate(observationPrefab, observationContainer);
                var text = entry.GetComponentInChildren<TextMeshProUGUI>();
                
                Debug.Log($"MemoryNoteButton: Adding observation: {observation.observationText}");
                text.text = observation.observationText;
                
                bool contradicted = _playerProfile.IsContradicted(observation.observationId);
                
                Debug.Log($"MemoryNoteButton: Observation '{observation.observationId}' contradicted state: {contradicted}");
                
                text.text = contradicted ? $"<color=#FF4444><u>{observation.observationText}</u></color>" : observation.observationText;

                // entry.GetComponent<ContradictionIcon>().SetActive(false);
            }
        }
        
        private void ClearUI()
        {
            titleText.text = "";
            notesText.text = "";
        }
    }
}
