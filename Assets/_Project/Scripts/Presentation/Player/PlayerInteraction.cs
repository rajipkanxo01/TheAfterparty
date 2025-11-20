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

        private void Awake()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            if (_gameStateService == null)
            {
                Debug.LogError("PlayerInteraction: GameStateService not found in ServiceLocator.");
            }
        }

        private void Start()
        {
            _dialogueControl = ServiceLocater.GetService<DialogueController>();
            if (_dialogueControl == null)
            {
                Debug.LogWarning("PlayerInteraction: DialogueController not found.");
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            // block interaction during transitions
            if (_gameStateService.IsState(GameState.Transition))
            {
                Debug.Log("PlayerInteraction: Interaction blocked during transition.");
                return;
            }

            // if in dialogue or cutscene, only advance dialogue
            if (_gameStateService.IsState(GameState.Dialogue) || _gameStateService.IsState(GameState.Cutscene))
            {
                _dialogueControl.ContinueDialogue();
                return;
            }

            if (_nearestInteractable == null)
                return;

            // detect destroyed interactables correctly
            if (_nearestInteractable is MonoBehaviour mono && mono == null)
            {
                Debug.LogWarning("PlayerInteraction: Interactable reference destroyed.");
                _nearestInteractable = null;
                return;
            }

            // safe interact call
            try
            {
                _nearestInteractable.Interact(gameObject);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"PlayerInteraction: Exception during interaction: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null)
            {
                Debug.LogWarning("PlayerInteraction: OnTriggerEnter2D received null collider.");
                return;
            }

            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                _nearestInteractable = interactable;
                Debug.Log($"[PlayerInteraction] Entered interactable trigger: {other.name}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null)
                return;

            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactable == _nearestInteractable)
                {
                    _nearestInteractable = null;
                    Debug.Log($"[PlayerInteraction] Exited interactable trigger: {other.name}");
                }
            }
        }

        private void OnDestroy()
        {
            _nearestInteractable = null;
        }
    }
}
