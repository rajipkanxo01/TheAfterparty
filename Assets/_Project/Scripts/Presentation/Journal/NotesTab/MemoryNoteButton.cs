using System;
using System.Linq;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    [Serializable]
    public class ObservationIcon
    {
        public Sprite iconImage;
        public Color color;
    }
    
    public class MemoryNoteButton : MonoBehaviour
    {
        [SerializeField] private string memoryID;
        [SerializeField] private string memoryTitle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Transform observationContainer;
        [SerializeField] private GameObject observationPrefab;

        [Header("Lock State")]
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private bool disableToggleInstead = true;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI titleText;

        
        [Header("Icons")]
        [SerializeField] private ObservationIcon contradictedIcon;
        [SerializeField] private ObservationIcon verifiedIcon;
        [SerializeField] private ObservationIcon unknownIcon;

        
        private PlayerProfile _playerProfile;
        private NotesYarnBridge _notesYarnBridge;
        private bool _isUnlocked;
        private ContradictionCounter _contradictionCounter;
        
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

            _notesYarnBridge = ServiceLocater.GetService<NotesYarnBridge>();
            
            if (_notesYarnBridge == null)
            {
                _notesYarnBridge = FindAnyObjectByType<NotesYarnBridge>();
            }
            
            _contradictionCounter = GetComponentInChildren<ContradictionCounter>();

            UpdateLockState();
        }

        private void OnEnable()
        {
            NotesEvent.OnNotesFound += HandleNewNoteFound;
        }

        private void HandleNewNoteFound(ObservationState arg1, string memoryId, string obsId)
        {
            if (memoryId == memoryID && toggle.isOn)
            {
                ShowNotesFor();
            }
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
            
            UIEvents.OnJournalOpen -= UpdateLockState;
            NotesEvent.OnNotesFound -= HandleNewNoteFound;
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
            Debug.Log("MemoryNoteButton: Showing notes for memory ID: " + memoryID);
            
            
            foreach (Transform child in observationContainer)
            {
                Destroy(child.gameObject);
            }
            
            if (_playerProfile == null)
                return;

            var memoryNotes = _playerProfile.AllNotes[memoryID];
            if (memoryNotes == null)
            {
                Debug.LogWarning($"MemoryNoteButton: No memory found for '{memoryID}'");
                _contradictionCounter.ClearUI();
                return;
            }

            titleText.text = memoryTitle;
            
            foreach (var observation in memoryNotes)
            {
                var observationState = observation.CurrentState;
    
                GameObject entry = Instantiate(observationPrefab, observationContainer);
                var text = entry.GetComponentInChildren<TextMeshProUGUI>();
                var iconObj = entry.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Icon"));
                var icon = iconObj?.GetComponent<Image>();

                var noteEntry = entry.GetComponent<JournalNoteEntry>();
                noteEntry.Init(observation.ObservationId, observation.MemoryId);

                if (text == null || icon == null)
                {
                    Debug.LogWarning("MemoryNoteButton: Observation prefab is missing TextMeshProUGUI or Image component.");
                    continue;
                }
                
                text.text = observation.NoteText;
    
                switch (observationState)
                {
                    case ObservationState.Verified:
                        icon.sprite = verifiedIcon.iconImage;
                        icon.color = verifiedIcon.color;
                        text.color = verifiedIcon.color;
                        break;
        
                    case ObservationState.Contradicted:
                        icon.sprite = contradictedIcon.iconImage;
                        icon.color = contradictedIcon.color;
                        text.color = contradictedIcon.color;
                        break;
        
                    case ObservationState.Unknown:
                    default:
                        icon.sprite = unknownIcon.iconImage;
                        icon.color = unknownIcon.color;
                        text.color = unknownIcon.color;
                        break;
                }
            }
            
            _contradictionCounter.UpdateContradictionCounts();
        }
        
    }
}
