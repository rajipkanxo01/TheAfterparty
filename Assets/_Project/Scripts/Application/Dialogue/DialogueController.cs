using System;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Clues;
using _Project.Scripts.Data.Npc;
using Unity.VisualScripting;

namespace _Project.Scripts.Application.Dialogue
{
    public enum DialogueType
    {
        NpcConversation, // Standard dialogue with NPCs
        PlayerMonologue, // When discovering clues
    }

    public class DialogueController : MonoBehaviour
    {
        [Header("Dialogue Settings")] 
        [SerializeField] private DialogueRunner runner;

        [SerializeField] private InMemoryVariableStorage variableStorage;
        [SerializeField] private NpcDatabase npcDatabase;

        private TaskCompletionSource<bool> _waitForContinue;
        private GameStateService _gameStateService;
        private PlayerProfile _playerProfile;
        private DialogueType _currentType = DialogueType.NpcConversation;
        private ClueManager _clueManager;
        
        private ClueData _currentClue;
        private bool _isClueDialogue = false;
        
        public DialogueType CurrentType => _currentType;

        private void Awake()
        {
            if (runner == null)
            {
                Debug.LogError("DialogueController: DialogueRunner not assigned.");
                return;
            }

            runner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();

            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);

            runner.AddCommandHandler("say", new Func<string[], Task>(HandleSayCommandAsync));
            
            ServiceLocater.RegisterService(this);
        }
        
        
        private void OnEnable()
        {
            ClueEvents.OnClueExamined += HandleClueExamined;
            //todo: maybe trigger dialogue for hint dialogue??
        }

        private void OnDisable()
        {
            ClueEvents.OnClueExamined -= HandleClueExamined;
        }

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _clueManager = ServiceLocater.GetService<ClueManager>();
        }
        
        public void StartDialogue(string nodeName, DialogueType type = DialogueType.NpcConversation)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                Debug.LogWarning("DialogueController: Yarn node name is null or empty.");
                return;
            }

            if (!runner.IsDialogueRunning)
                Debug.Log($"DialogueController: Starting dialogue node '{nodeName}'");

            _currentType = type;
            DialogueEvents.RaiseDialogueStarted();
            runner.StartDialogue(nodeName);
        }

        private void HandleClueExamined(ClueData clueData)
        {
            StartDialogue(clueData.dialogueNode, DialogueType.PlayerMonologue);
            _isClueDialogue = true;
            _currentClue = clueData;
        }
        
        private void OnNodeStart(string nodeName)
        {
            // Enter dialogue game state
            _gameStateService?.SetState(GameState.Dialogue);
        }

        private void OnDialogueComplete()
        {
            // Return to normal gameplay
            _gameStateService?.SetState(GameState.Normal);
            _currentType = DialogueType.NpcConversation;
            DialogueEvents.RaiseDialogueEnded();
            
            if (_isClueDialogue)
            {
                Debug.Log($"DialogueController: Dialogue completed for clue {_currentClue.clueId}");
                _clueManager.CompleteClue(_currentClue.clueId);
                _isClueDialogue = false;
                _currentClue = null;
            }
        }
        
        private async Task HandleSayCommandAsync(string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogWarning("DialogueController: Invalid 'say' command parameters.");
                return;
            }

            if (npcDatabase == null)
            {
                Debug.LogError("DialogueController: NPC database not assigned!");
                return;
            }

            string speakerId = parameters[0];
            string line = parameters[1];

            var (displayName, portrait) = ResolveSpeaker(speakerId);
            DialogueEvents.RaiseDialogueLineStarted(this,
                new DialogueLineEventArgs(displayName, line, portrait, speakerId));

            _waitForContinue = new TaskCompletionSource<bool>();
            await _waitForContinue.Task;

            DialogueEvents.RaiseDialogueContinued();
        }

        public void ContinueDialogue()
        {
            if (_waitForContinue != null && !_waitForContinue.Task.IsCompleted)
            {
                _waitForContinue.SetResult(true);
            }
        }

        
        private (string name, Sprite portrait) ResolveSpeaker(string speakerId)
        {
            if (string.IsNullOrWhiteSpace(speakerId))
                return ("Unknown", null);

            string id = speakerId.Trim().ToLowerInvariant();

            // Player identifiers
            if (id == "detectivecat")
            {
                if (_playerProfile != null)
                    return (_playerProfile.DisplayName, _playerProfile.Portrait);

                return ("Player", null);
            }

            // NPCs
            var npc = npcDatabase.GetById(speakerId);
            if (npc != null)
                return (npc.npcName, npc.portrait);

            // Fallbacks by dialogue type
            return _currentType switch
            {
                DialogueType.PlayerMonologue when _playerProfile != null =>
                    (_playerProfile.DisplayName, _playerProfile.Portrait),

                DialogueType.PlayerMonologue =>
                    ("Player", null),

                _ => (speakerId, null)
            };
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