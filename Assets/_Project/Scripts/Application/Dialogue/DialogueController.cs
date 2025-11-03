using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Npc;
using UnityEngine;
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
        public DialogueType CurrentType => _currentType;
        public NpcDialogueHandler NpcHandler => _npcHandler;

        private void Start()
        {
            if (!runner)
            {
                Debug.LogError("DialogueController: DialogueRunner not assigned.");
                return;
            }

            runner.DialoguePresenters = System.Array.Empty<DialoguePresenterBase>();
            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);
            
            _gameState = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _npcDatabase = ServiceLocater.GetService<NpcDatabase>();

            _speakerResolver = new DialogueSpeakerResolver(_npcDatabase);
            _commandRegistry = new DialogueCommandRegistry(runner);

            _clueHandler = new ClueDialogueHandler(this);
            _npcHandler = new NpcDialogueHandler(this, _npcDatabase);

            ServiceLocater.RegisterService(this);
            ServiceLocater.RegisterService(_speakerResolver);
            
            _commandRegistry.RegisterBuiltInCommands();
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
        }

        private void OnNodeStart(string nodeName)
        {
            _gameState?.SetState(GameState.Dialogue);
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