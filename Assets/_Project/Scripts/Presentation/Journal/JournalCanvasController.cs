using _Project.Scripts.Application.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private PlayerInput journalInput; 
        
        private PlayerInput _playerInput;      

        private void Awake()
        {
            menu.SetActive(false);
            journalInput.enabled = false;

            if (SceneManager.GetActiveScene().name == "MainMenuScene")
            {
                _playerInput = null;
            }
            else
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    _playerInput = player.GetComponent<PlayerInput>();
                }
            }
        }

        private void OnEnable()
        {
            UIEvents.OnJournalOpen += Show;
            UIEvents.OnJournalClose += Hide;
        }

        private void OnDisable()
        {
            UIEvents.OnJournalOpen -= Show;
            UIEvents.OnJournalClose -= Hide;
        }

        private void Show()
        {
            menu.SetActive(true);

            if (_playerInput != null)
            {
                _playerInput.enabled = false;
            }

            journalInput.enabled = true;
        }

        private void Hide()
        {
            menu.SetActive(false);

            journalInput.enabled = false;

            if (_playerInput != null)
                _playerInput.enabled = true;
        }
    }
}