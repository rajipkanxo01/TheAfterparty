using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;
using UnityEngine;

namespace _Project.Scripts.Application
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private ClueDatabase clueDatabase;
        // [SerializeField] private NpcDatabase npcDatabase;
        
        private void Awake()
        {
            var gameStateService = new GameStateService();
            ServiceLocater.RegisterService(gameStateService);
            
            var clueManager = new ClueManager(clueDatabase);
            ServiceLocater.RegisterService(clueManager);
        }
    }
}