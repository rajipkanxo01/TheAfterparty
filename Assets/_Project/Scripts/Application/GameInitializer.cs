using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Clues;
using _Project.Scripts.Data.Player;
using UnityEngine;

namespace _Project.Scripts.Application
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerProfileSo playerProfile;
        
        [Header("Databases")]
        [SerializeField] private ClueDatabase clueDatabase;
        // [SerializeField] private NpcDatabase npcDatabase;
        
        private void Awake()
        {
            var gameStateService = new GameStateService();
            ServiceLocater.RegisterService(gameStateService);
            
            var clueManager = new ClueManager(clueDatabase);
            ServiceLocater.RegisterService(clueManager);
            
            var clueService = new ClueService(clueManager, gameStateService);
            ServiceLocater.RegisterService(clueService);

            var profile = new PlayerProfile(playerProfile.displayName, playerProfile.playerId, playerProfile.portrait);
            ServiceLocater.RegisterService(profile);
        }
    }
}