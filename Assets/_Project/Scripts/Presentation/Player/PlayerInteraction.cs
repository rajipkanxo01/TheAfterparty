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

            _dialogueControl = FindAnyObjectByType<DialogueController>();
            if (_dialogueControl == null)
            {
                Debug.LogWarning("PlayerInteraction: DialogueController not found at Awake. It may be created later.");
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (_gameStateService == null)
            {
                Debug.LogError("PlayerInteraction: GameStateService is missing. Interaction aborted.");
                return;
            }
            
            if (_gameStateService.IsState(GameState.Transition)) return;

            if (_gameStateService.IsState(GameState.Dialogue) || _gameStateService.IsState(GameState.Cutscene))
            {
                if (_dialogueControl == null)
                {
                    _dialogueControl = FindAnyObjectByType<DialogueController>();
                    if (_dialogueControl == null)
                    {
                        Debug.LogWarning("PlayerInteraction: DialogueController not found during dialogue state.");
                        return;
                    }
                }

                _dialogueControl.ContinueDialogue();
                return;
            }

            if (_nearestInteractable == null)
            {
                return;
            }

            if (_nearestInteractable is MonoBehaviour mono && mono == null)
            {
                Debug.LogWarning("PlayerInteraction: Interactable reference destroyed.");
                _nearestInteractable = null;
                return;
            }

            try
            {
                _nearestInteractable.Interact(gameObject);
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayerInteraction: Exception during interaction: {ex.Message}\n{ex.StackTrace}");
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
