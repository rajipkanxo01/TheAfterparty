using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private IInteractable _nearestInteractable;

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Debug.Log("PlayerInteraction: OnInteract triggered");

            if (_nearestInteractable != null)
            {
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