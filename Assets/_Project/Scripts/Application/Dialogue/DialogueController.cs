using System;
using System.Threading.Tasks;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;
using Yarn.Unity;
using _Project.Scripts.Data.Npc;

namespace _Project.Scripts.Application.Dialogue
{
    public enum DialogueType
    {
        NpcConversation,  // Standard dialogue with NPCs
        PlayerMonologue,  // When discovering clues
    }
    
    public class DialogueController : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private InMemoryVariableStorage variableStorage;
        [SerializeField] private NpcDatabase npcDatabase;
        
        
        /*public event EventHandler<DialogueLineEventArgs> OnDialogueLineStarted;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action OnDialogueContinued;*/
        
        private TaskCompletionSource<bool> _waitForContinue;
        private GameStateService _gameStateService;
        private DialogueType _currentDialogueType  = DialogueType.NpcConversation;
        private PlayerProfile _playerProfile;
        
        public DialogueType CurrentDialogueType => _currentDialogueType;

        private void Awake()
        {
            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);
            runner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();
            runner.AddCommandHandler("say", new Func<string[], Task>(HandleSayCommandAsync));
            
            
            // todo: this is temp for debugging
            ClueEvents.OnClueExamined += (clue) =>
            {
                Debug.Log($"{clue} examined");
            };
            
            ServiceLocater.RegisterService(this);
        }

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }
        
        public void StartDialogue(string yarnNode, DialogueType dialogueType = DialogueType.NpcConversation)
        {
            if (string.IsNullOrEmpty(yarnNode))
            {
                Debug.LogWarning("DialogueController: No Yarn node provided.");
                return;
            }
            
            if (!runner.IsDialogueRunning)
                Debug.Log($"Starting dialogue node: {yarnNode}");

            _currentDialogueType = dialogueType;
            DialogueEvents.RaiseDialogueStarted();
            runner.StartDialogue(yarnNode);
        }
        
        private async Task HandleSayCommandAsync(string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogWarning("HandleSayCommand: Invalid parameters passed.");
                return;
            }

            if (npcDatabase == null)
            {
                Debug.LogError("HandleSayCommand: npcDatabase not assigned!");
                return;
            }

            string speakerId = parameters[0];
            string line = parameters[1];

            var (displayName, portrait) = ResolveSpeaker(speakerId);
            DialogueEvents.RaiseDialogueLineStarted(this, new DialogueLineEventArgs(displayName, line, portrait, speakerId));
            
            _waitForContinue = new TaskCompletionSource<bool>();
            await _waitForContinue.Task;
            
            DialogueEvents.RaiseDialogueContinued();
        }

        private (string, Sprite) ResolveSpeaker(string speakerId)
        {
            switch (_currentDialogueType)
            {
                case DialogueType.NpcConversation:
                    var npc = npcDatabase.GetById(speakerId);
                    if (npc == null)
                    {
                        Debug.LogWarning($"DialogueController: NPC '{speakerId}' not found in database.");
                        return (speakerId, null);
                    }
                    return (npc.npcName, npc.portrait);
                
                case DialogueType.PlayerMonologue:
                    return (_playerProfile.DisplayName, _playerProfile.Portrait);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnNodeStart(string nodeName)
        {
            switch (_currentDialogueType)
            {
                case DialogueType.PlayerMonologue:
                case DialogueType.NpcConversation:
                    _gameStateService.SetState(GameState.Dialogue);
                    break;
                default:
                    Debug.LogWarning($"DialogueController: Unknown dialogue type '{_currentDialogueType}'.");
                    break;
            }
        }

        private void OnDialogueComplete()
        {
            _gameStateService.SetState(GameState.Normal);
            _currentDialogueType = DialogueType.NpcConversation;
            DialogueEvents.RaiseDialogueEnded();
        }

        public void ContinueDialogue()
        {
            if (_waitForContinue != null && !_waitForContinue.Task.IsCompleted)
            {
                _waitForContinue.SetResult(true);
            }
        }

        public int GetNpcProgress(string npcId)
        {
            if (variableStorage.TryGetValue($"${npcId}_progress", out float value))
            {
                return Mathf.RoundToInt(value);
            }
            return 0;
        }
    }
}
