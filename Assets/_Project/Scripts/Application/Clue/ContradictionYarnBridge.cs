using System;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;
using Yarn.Unity;

namespace _Project.Scripts.Application.Clue
{
    public class ContradictionYarnBridge : MonoBehaviour
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
            
            ContradictionEvents.OnContradictionFound += HandleContradictionFound;
        }

        private void OnDestroy()
        {
            ContradictionEvents.OnContradictionFound -= HandleContradictionFound;
        }

        private void HandleContradictionFound(string observationId)
        {
            if (variableStorage is null)
            {
                Debug.LogError("ContradictionYarnBridge: VariableStorage not found.");
                return;
            }
            
            string flagVar = $"$contradiction_{observationId}";
            variableStorage.SetValue(flagVar, true);

            var observationStates = _playerProfile.GetObservationStates();
            
            float newCount;
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