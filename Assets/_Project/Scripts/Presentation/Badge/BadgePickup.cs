using _Project.Scripts.Application.Badge;
using UnityEngine;

namespace _Project.Scripts.Presentation.Badge
{
    public class BadgePickup : MonoBehaviour, IInteractable
    {
        public void Interact(GameObject interactor)
        {
            BadgeEvents.RaiseBadgePicked();

            Destroy(gameObject);
        }

        public string GetInteractionPrompt()
        {
            return "Pick up Badge";
        }
    }
}