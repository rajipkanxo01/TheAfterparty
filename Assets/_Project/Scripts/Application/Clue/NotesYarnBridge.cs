using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;
using Yarn.Unity;

namespace _Project.Scripts.Application.Clue
{
    public class NotesYarnBridge : MonoBehaviour
    {
        [SerializeField] private InMemoryVariableStorage variableStorage;
        
        private PlayerProfile _playerProfile;

        private void Start()
        {
            variableStorage ??= FindAnyObjectByType<InMemoryVariableStorage>();
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("ContradictionYarnBridge: PlayerProfile not found.");
            }
            
            NotesEvent.OnNotesFound += HandleNotesFound;
        }

        private void OnDestroy()
        {
            NotesEvent.OnNotesFound -= HandleNotesFound;
        }

        private void HandleNotesFound(ObservationState state, string memoryId, string observationId)
        {
            if (variableStorage is null)
            {
                Debug.LogError("ContradictionYarnBridge: VariableStorage not found.");
                return;
            }
            
            string flagVar = $"$contradiction_{observationId}";
            variableStorage.SetValue(flagVar, true);
            
            float newCount;
            if (state.Equals(ObservationState.Contradicted))
            {
                if (variableStorage.TryGetValue("$contradictions_found", out float current))
                {
                    newCount = current + 1f;
                }
                else
                {
                    // First contradiction ever found
                    newCount = 1f;
                }
                
                variableStorage.SetValue("$contradictions_found", newCount);
            }

        }
    }
}