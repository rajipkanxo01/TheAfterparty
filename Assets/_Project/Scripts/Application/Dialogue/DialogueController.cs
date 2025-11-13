using System;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue.Handlers;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Npc;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace _Project.Scripts.Application.Dialogue
{
    public enum DialogueType
    {
        NpcConversation,
        PlayerMonologue,
    }

    public class DialogueController : MonoBehaviour
    {
        [Header("Dialogue Settings")] 
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private float autoAdvanceDelay = 1.5f;
        [SerializeField] private InMemoryVariableStorage variableStorage;

        private DialogueCommandRegistry _commandRegistry;
        private DialogueSpeakerResolver _speakerResolver;
        private ClueDialogueHandler _clueHandler;
        private NpcDialogueHandler _npcHandler;
        private GameStateService _gameState;
        private NpcDatabase _npcDatabase;
        private PlayerProfile _playerProfile;

        private DialogueType _currentType = DialogueType.NpcConversation;
        private TaskCompletionSource<bool> _waitForContinue;
        private bool _autoModeEnabled = false;
        public DialogueType CurrentType => _currentType;
        public NpcDialogueHandler NpcHandler => _npcHandler;
        public bool IsAutoModeEnabled => _autoModeEnabled;
        public float AutoAdvanceDelay => autoAdvanceDelay;
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }


        private void Start()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            
            if (!runner)
            {
                Debug.LogError("DialogueController: DialogueRunner not assigned.");
                return;
            }

            runner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();
            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onNodeComplete?.AddListener(HandleNodeComplete);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);

            _gameState = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _npcDatabase = ServiceLocater.GetService<NpcDatabase>();

            _speakerResolver = new DialogueSpeakerResolver(_npcDatabase);
            _commandRegistry = new DialogueCommandRegistry(runner);

            _clueHandler = new ClueDialogueHandler(this);
            _npcHandler = new NpcDialogueHandler(this, _npcDatabase);

            // ServiceLocater.RegisterService(this);
            ServiceLocater.RegisterService(_speakerResolver);

            _commandRegistry.RegisterBuiltInCommands();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (runner == null)
            {
                runner = FindAnyObjectByType<DialogueRunner>();
                if (runner != null)
                {
                    Debug.Log($"DialogueController: Found new DialogueRunner in scene '{scene.name}'.");
                }
                else
                {
                    Debug.LogWarning($"DialogueController: No DialogueRunner found in scene '{scene.name}'.");
                }
            }
        }

        private async void HandleNodeComplete(string nodeName)
        {
            if (_autoModeEnabled)
            {
                await Task.Delay((int)(autoAdvanceDelay * 1000f));

                if (runner != null && runner.Dialogue.IsActive)
                {
                    runner.Dialogue.Continue();
                }

                _autoModeEnabled = false;
            }
        }

        public void StartDialogue(string nodeName, DialogueType type = DialogueType.NpcConversation)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                Debug.LogWarning("DialogueController: Yarn node name missing.");
                return;
            }

            _currentType = type;
            DialogueEvents.RaiseDialogueStarted();
            runner.StartDialogue(nodeName);
            
            Debug.Log($"DialogueController: StartDialogue node '{nodeName}'.");
        }

        private void OnNodeStart(string nodeName)
        {
            // _gameState?.SetState(GameState.Dialogue);
        }

        private void OnDialogueComplete()
        {
            _gameState?.SetState(GameState.Normal);
            _currentType = DialogueType.NpcConversation;
            DialogueEvents.RaiseDialogueEnded();

            _clueHandler.HandleDialogueEnd();
            _npcHandler.HandleConversationEnd();
        }


        public void ContinueDialogue()
        {
            DialogueEvents.RaiseDialogueContinueRequested();
        }
        
        public void ChangeAutoModeTo(bool mode)
        {
            _autoModeEnabled = mode;
        }
        
        public int GetNpcProgress(string npcId)
        {
            if (string.IsNullOrEmpty(npcId) || variableStorage == null)
                return 0;

            return variableStorage.TryGetValue($"${npcId}_progress", out float value)
                ? Mathf.RoundToInt(value)
                : 0;
        }
    }
}