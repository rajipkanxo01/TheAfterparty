using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;
using Yarn.Unity;

namespace _Project.Scripts.Application.Clue
{
    public class NotesYarnBridge : MonoBehaviour
    {
        [SerializeField] private InMemoryVariableStorage variableStorage;
        [SerializeField] private DialogueRunner dialogueRunner;
        
        private PlayerProfile _playerProfile;

        private void Awake()
        {
            ServiceLocater.RegisterService(this);
        }

        private void Start()
        {
            variableStorage ??= FindAnyObjectByType<InMemoryVariableStorage>();
            dialogueRunner ??= FindAnyObjectByType<DialogueRunner>();
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("ContradictionYarnBridge: PlayerProfile not found.");
            }
            
            NotesEvent.OnNotesFound += HandleNotesFound;
            
            // Register Yarn functions
            if (dialogueRunner != null)
            {
                dialogueRunner.AddFunction<string, int>("get_selected_contradictions", (memoryId) => GetSelectedContradictionsCount(memoryId));
                dialogueRunner.AddFunction<string, string, bool>("is_contradiction_selected", (memoryId, observationId) => IsContradictionSelected(memoryId, observationId));
            }
        }

        private void OnDestroy()
        {
            NotesEvent.OnNotesFound -= HandleNotesFound;
        }

        private void HandleNotesFound(ObservationState state, string memoryId, string observationId)
        {
            if (variableStorage is null)
            {
                Debug.LogError("NotesYarnBridge: VariableStorage not found.");
                return;
            }
            
            string flagVar = $"$notes_{memoryId}_{observationId}";
            variableStorage.SetValue(flagVar, true);
            
            float newCount;
            if (state.Equals(ObservationState.Contradicted))
            {
                if (variableStorage.TryGetValue($"${memoryId}_contradictions_found", out float current))
                {
                    newCount = current + 1f;
                }
                else
                {
                    // First contradiction ever found
                    newCount = 1f;
                }

                variableStorage.SetValue($"${memoryId}_contradictions_found", newCount);
            }
        }

        public float GetVariableValue(string variableName, float defaultValue = 0f)
        {
            if (variableStorage == null)
            {
                Debug.LogError("NotesYarnBridge: VariableStorage not found.");
                return defaultValue;
            }

            
            Debug.Log("NotesYarnBridge: Retrieving variable '" + variableName + "'");
            if (variableStorage.TryGetValue(variableName, out float value))
            {
                return value;
            }

            return defaultValue;
        }

        public bool GetVariableValue(string variableName, bool defaultValue = false)
        {
            if (variableStorage == null)
            {
                Debug.LogError("NotesYarnBridge: VariableStorage not found.");
                return defaultValue;
            }

            if (variableStorage.TryGetValue(variableName, out bool value))
            {
                return value;
            }

            return defaultValue;
        }
        
        // Yarn function to get the number of selected contradictions for a memory
        public int GetSelectedContradictionsCount(string memoryId)
        {
            if (_playerProfile == null)
            {
                Debug.LogWarning("NotesYarnBridge: PlayerProfile not found. Cannot get selected contradictions count.");
                return 0;
            }
            
            int count = _playerProfile.GetSelectedContradictionsCount(memoryId);
            Debug.Log($"NotesYarnBridge: Memory '{memoryId}' has {count} selected contradictions.");
            return count;
        }
        
        // Yarn function to check if a specific contradiction is selected
        public bool IsContradictionSelected(string memoryId, string observationId)
        {
            if (_playerProfile == null)
            {
                Debug.LogWarning("NotesYarnBridge: PlayerProfile not found. Cannot check if contradiction is selected.");
                return false;
            }
            
            bool isSelected = _playerProfile.IsContradictionSelected(memoryId, observationId);
            Debug.Log($"NotesYarnBridge: Contradiction '{observationId}' in memory '{memoryId}' is {(isSelected ? "selected" : "not selected")}.");
            return isSelected;
        }
    }
}