using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcDialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcId;
        [SerializeField] private Transform headPoint;

        private void Start()
        {
           
        }

        public void Interact(GameObject interactor)
        {
            Debug.Log($"NpcDialogueTrigger: Interact with NPC {npcId}");
        }

        public string GetInteractionPrompt()
        {
            return "Talk";
        }
    }
}