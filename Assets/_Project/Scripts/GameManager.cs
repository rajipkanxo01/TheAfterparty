using _Project.Scripts.Application;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Presentation.Dialogue;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DialogueView dialogueView;

        private DialogueService _dialogueService;
        
        private void Awake()
        {
            // Create and register the DialogueService once for the scene
            _dialogueService = new DialogueService();

            // Initialize the DialogueView with this service
            dialogueView.Initialize(_dialogueService);

            ServiceLocater.RegisterService(_dialogueService);
            ServiceLocater.RegisterService(dialogueView);
        }
    }
}