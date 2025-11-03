using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private IInteractable _nearestInteractable;
        private DialogueController _dialogueControl;
        private GameStateService _gameStateService;

        private void Start()
        {
            _dialogueControl = FindAnyObjectByType<DialogueController>();
            if (_dialogueControl == null)
            {
                Debug.LogError("PlayerInteraction: DialogueController not found in scene.");
            }
            
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!_gameStateService.IsState(GameState.Normal) && !_gameStateService.IsState(GameState.Dialogue)) return;

            if (_nearestInteractable != null)
            {
                if (_gameStateService.IsState(GameState.Dialogue))
                {
                    _dialogueControl.ContinueDialogue();
                    return;
                }
                
                _nearestInteractable.Interact(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                Debug.Log("PlayerInteraction: Enter interactable trigger" + other.name);
                _nearestInteractable = interactable;
                // maybe show prompt like "Press E to interact"
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                Debug.Log("PlayerInteraction: Exit interactable trigger" + other.name);
                _nearestInteractable = null;
                // hide prompt
            }
        }
    }
}