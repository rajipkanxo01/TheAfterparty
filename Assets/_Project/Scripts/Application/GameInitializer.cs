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
        
        [Header("Configs")]
        [SerializeField] private SniffConfig sniffConfig;
        
        private PlayerProfileListener _profileListener;
        
        private void Awake()
        {
            // register databases
            ServiceLocater.RegisterService(clueDatabase);
            // ServiceLocater.RegisterService(npcDatabase);
            
            // create and register player profile
            var profile = new PlayerProfile(playerProfile.displayName, playerProfile.playerId, playerProfile.portrait);
            ServiceLocater.RegisterService(profile);
            
            // create and register services and managers
            var gameStateService = new GameStateService();
            ServiceLocater.RegisterService(gameStateService);
            
            var clueManager = new ClueManager(clueDatabase);
            ServiceLocater.RegisterService(clueManager);
            
            
            var clueService = new ClueService(clueManager, gameStateService, sniffConfig);
            ServiceLocater.RegisterService(clueService);
            


            // create and register profile listener
            _profileListener = new PlayerProfileListener(profile);
        }
        
        private void OnDestroy()
        {
            _profileListener.Unsubscribe();
        }
    }
}