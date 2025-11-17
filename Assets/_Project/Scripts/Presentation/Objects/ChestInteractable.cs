using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Objects;
using UnityEngine;

namespace _Project.Scripts.Presentation.Objects
{
    public class ChestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private ChestData chestData;

        private DialogueController _dialogueController;
        private GameStateService _gameStateService;
        private PlayerProfile _playerProfile;

        private bool _codeKnown;

        private void Start()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();

            // Load persistent state from PlayerProfile
            _codeKnown = _playerProfile.HasFlag($"ChestCode_{chestData.correctCode}");
        }
        
        public void Interact(GameObject interactor)
        {
            if (!_codeKnown)
            {
                ToastNotification.Show("Mogens doesn't know the code for this chest yet.");
                return;
            }

            _gameStateService.SetState(GameState.Paused);
            ChestUI.Open(chestData);
        }

        public string GetInteractionPrompt()
        {
            return "Interact with Chest";
        }

        private void UnlockChest()
        {
            _codeKnown = true;
            _playerProfile.SetFlag($"ChestCode_{chestData.correctCode}", true);
        }
    }
}
