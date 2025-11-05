using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Clues;
using _Project.Scripts.Data.Npc;
using _Project.Scripts.Data.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Application
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerProfileSo playerProfile;

        [Header("Databases")]
        [SerializeField] private ClueDatabase clueDatabase;
        [SerializeField] private NpcDatabase npcDatabase;

        [Header("Configs")]
        [SerializeField] private SniffConfig sniffConfig;

        [Header("Scene Systems (to enable after Start)")]
        [SerializeField] private GameObject[] systemsToEnable;

        private static bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            InitializeServices();
        }

        private void InitializeServices()
        {
            Debug.Log("[GameInitializer] Initializing services...");

            ServiceLocater.RegisterService(clueDatabase);
            ServiceLocater.RegisterService(npcDatabase);

            var profile = new PlayerProfile(playerProfile.displayName, playerProfile.playerId, playerProfile.portrait);
            ServiceLocater.RegisterService(profile);

            var gameStateService = new GameStateService();
            ServiceLocater.RegisterService(gameStateService);

            var clueManager = new ClueManager(clueDatabase);
            ServiceLocater.RegisterService(clueManager);

            var memoryManager = new MemoryManager();
            ServiceLocater.RegisterService(memoryManager);

            var clueService = new ClueService(clueManager, gameStateService, sniffConfig);
            ServiceLocater.RegisterService(clueService);
            
            var dialogueController = FindAnyObjectByType<DialogueController>();
            if (dialogueController != null)
            {
                ServiceLocater.RegisterService(dialogueController);
            }
        }
    }
}
