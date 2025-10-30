using System;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;
using UnityEngine;

namespace _Project.Scripts.Presentation.Clues
{
    public class ClueObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string clueId;
        
        private ClueService _clueService;
        
        private void Start()
        {
            _clueService = ServiceLocater.GetService<ClueService>();
            if (_clueService == null)
            {
                Debug.LogError("ClueView: ClueService not found in scene.");
            }
        }

        private void OnEnable()
        {
            ClueEvents.OnClueDiscovered += HandleClueDiscovered;
        }

        private void OnDisable()
        {
            ClueEvents.OnClueDiscovered -= HandleClueDiscovered;
        }

        private void HandleClueDiscovered(ClueData discoveredClue)
        {
            if (discoveredClue.clueId == clueId)
            {
                Destroy(gameObject);
                ToastNotification.Show( $"Clue '{discoveredClue.clueName}' added to journal.", "success" );
                // gameObject.SetActive(false);
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.CompareTag("Player")) return;
            
            _clueService.Examine(clueId);
        }

        public string GetInteractionPrompt()
        {
            return "Examine Clue";
        }
    }
}