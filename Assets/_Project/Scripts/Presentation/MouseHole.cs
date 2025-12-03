using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation
{
    public class MouseHole : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;
        [SerializeField] private string requiredMemoryID;
        
        private PlayerProfile _playerProfile;
        
        private void Start()
        {
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("MouseHole: PlayerProfile not found.");
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_playerProfile.HasFragmentCompletedMemory(requiredMemoryID))
            {
                if (other.CompareTag("Player"))
                {
                    DialogueEvents.RaiseDialogueStart("Mouse_Locked", DialogueType.PlayerMonologue);
                    return;
                }
            }
            
            if (other.CompareTag("Player"))
            {
                LoadScene();
            }
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}