using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Presentation.Journal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerJournal : MonoBehaviour
    {
        private PlayerJournalManager playerJournalManager;
        [SerializeField] private GameObject journalPanel;
        private Image journalImage;
        private JournalManager journalManager;

        private void Start()
        {
            if (journalPanel.TryGetComponent<JournalManager>(out journalManager))
            {
                playerJournalManager = new PlayerJournalManager(journalManager);
                playerJournalManager.ToggleJournal += SetJournal;
            }
            else
            {
                Debug.LogError("Could not find JournalManager in journalPanel");
            }

            journalImage = journalPanel.GetComponent<Image>();
        }

        public void JournalInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerJournalManager.ProcessJournalInput();
        }
        
        public void SetJournal(bool open)
        {
            journalManager.ToggleClues(open);
        }
    }
}