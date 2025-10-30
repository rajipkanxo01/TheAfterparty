using System;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using UnityEngine;

namespace _Project.Scripts.Presentation.Clues
{
    public class ClueView : MonoBehaviour, IInteractable
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