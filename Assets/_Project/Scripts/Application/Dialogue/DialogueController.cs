using System;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using UnityEngine;
using Yarn.Unity;
using _Project.Scripts.Data.Npc;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private InMemoryVariableStorage variableStorage;
        [SerializeField] private NpcDatabase npcDatabase;

        public event EventHandler<DialogueLineEventArgs> OnDialogueLineStarted;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action OnDialogueContinued;
        

        private TaskCompletionSource<bool> _waitForContinue;
        private GameStateService _gameStateService;
        public bool IsDialogueRunning { get; private set; }

        private void Awake()
        {
            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);
            runner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();
            runner.AddCommandHandler("say", new Func<string[], Task>(HandleSayCommandAsync));
            
            ServiceLocater.RegisterService(this);
        }

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        public void StartDialogueWithNpc(string npcId)
        {
            var npc = npcDatabase.GetById(npcId);
            if (npc == null)
            {
                Debug.LogWarning($"NPC '{npcId}' not found.");
                return;
            }

            IsDialogueRunning = true;
            OnDialogueStarted?.Invoke();
            runner.StartDialogue(npc.yarnRootNode);
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

            string speaker = parameters[0];
            string line = parameters[1];

            var npc = npcDatabase.GetById(speaker);
            if (npc == null)
            {
                Debug.LogWarning($"HandleSayCommand: NPC '{speaker}' not found in database.");
            }

            Sprite portrait = npc != null ? npc.portrait : null;
            string speakerId = npc != null ? npc.npcId : speaker.ToLower();

            OnDialogueLineStarted?.Invoke(this, new DialogueLineEventArgs(speaker, line, portrait, speakerId));

            _waitForContinue = new TaskCompletionSource<bool>();

            await _waitForContinue.Task;
            OnDialogueContinued?.Invoke();
        }

        private void OnNodeStart(string nodeName)
        {
            _gameStateService.SetState(GameState.Dialogue);
        }

        private void OnDialogueComplete()
        {
            _gameStateService.SetState(GameState.Normal);
            IsDialogueRunning = false;
            OnDialogueEnded?.Invoke();
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
