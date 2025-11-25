using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Events;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    public class NotesPageController : MonoBehaviour
    {
        [SerializeField] private List<MemoryNoteButton> noteButtons;

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
            Debug.Log($"NotesPageController: Page changed to index {index}. Expecting {NotesTabIndex} for Notes tab.");
            if (index != NotesTabIndex)
            {
                return;
            }

            RefreshButtonStates();
            SelectFirstUnlocked();
        }

        private void RefreshButtonStates()
        {
            foreach (var btn in noteButtons)
            {
                btn.UpdateLockState();
            }
        }

        private void SelectFirstUnlocked()
        {
            foreach (var btn in noteButtons.Where(b => b.IsUnlocked))
            {
                btn.SelectWithoutNotify();
                btn.ShowNotesFor();
                Debug.Log("NotesPageController: Selected first unlocked memory note.");
                return;
            }
        }
    }
}