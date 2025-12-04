using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Player;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class NotesPageController : MonoBehaviour
    {
        [SerializeField] private List<MemoryNoteButton> noteButtons;
        
        private int currentIndex = 0;

        private const int NotesTabIndex = 1;

        private void Awake()
        {
            if (noteButtons == null || noteButtons.Count == 0)
            {
                noteButtons = GetComponentsInChildren<MemoryNoteButton>(true).ToList();
            }
        }

        private void Start()
        {
            UIEvents.OnJournalTabChanged += OnPageChanged;
        }

        private void OnDestroy()
        {
            UIEvents.OnJournalTabChanged -= OnPageChanged;
        }

        private void OnPageChanged(int index)
        {
            if (index != NotesTabIndex)
            {
                return;
            }

            RefreshButtonStates();
        }

        private void RefreshButtonStates()
        {
            foreach (var btn in noteButtons)
            {
                btn.UpdateLockState();
            }
        }
        
        public void OnNoteClicked(string memoryId, string observationId, JournalNoteEntry entry)
        {
            Debug.Log("NotesPageController: Note clicked: " + observationId);
        }


        public void MoveSelection(int direction)
        {
            /*// Find only unlocked buttons
            var unlockedButtons = noteButtons.Where(btn => btn != null && btn.IsUnlocked).ToList();
            
            if (unlockedButtons.Count == 0)
                return;

            currentIndex = Mathf.Clamp(currentIndex + direction, 0, unlockedButtons.Count - 1);
            
            // You could add visual highlighting here if needed
            // For now, this just updates the index for selection*/
        }

        public void SelectCurrent()
        {
            /*var unlockedButtons = noteButtons.Where(btn => btn != null && btn.IsUnlocked).ToList();
            
            if (unlockedButtons.Count == 0)
                return;

            if (currentIndex >= 0 && currentIndex < unlockedButtons.Count)
            {
                // Simulate clicking the button by setting its toggle to true
                var targetButton = unlockedButtons[currentIndex];
                var toggle = targetButton.GetComponent<UnityEngine.UI.Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }*/
        }
    }
}