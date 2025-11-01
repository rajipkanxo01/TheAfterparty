using System;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Player
{
    public class SniffInputHandler : MonoBehaviour
    {
        [SerializeField] private SniffConfig sniffConfig;
        [SerializeField] private bool showDebugRadius = true;
        [SerializeField] private Transform playerTransform;
        
        private ClueService _clueService;
        
        private void Start()
        {
            _clueService = ServiceLocater.GetService<ClueService>();
        }

        private void OnEnable()
        {
            ClueEvents.OnHintFound += HandleHintFound;
        }
        
        private void OnDisable()
        {
            ClueEvents.OnHintFound -= HandleHintFound;
        }
        
        public void OnSniffPerformed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            _clueService.PerformSniff(playerTransform.position);
            
        }
        
        private void HandleHintFound(ClueData clueData)
        {
            // Additional handling if needed when a hint is found
            Debug.Log($"Hint found with dialogue node: {clueData.clueId}");
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugRadius || _clueService == null) return;

            Gizmos.color = Color.gold;
            Gizmos.DrawWireSphere(playerTransform.position, sniffConfig.SniffRadius);
        }
#endif
    }
}