using System;
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

        private void Awake()
        {
            runner.onNodeStart?.AddListener(OnNodeStart);
            runner.onDialogueComplete?.AddListener(OnDialogueComplete);
            runner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();
            runner.AddCommandHandler("say", new Action<string[]>(HandleSayCommand));
            
            ServiceLocater.RegisterService(this);
        }

        public void StartDialogueWithNpc(string npcId)
        {
            var npc = npcDatabase.GetById(npcId);
            if (npc == null)
            {
                Debug.LogWarning($"NPC '{npcId}' not found.");
                return;
            }

            OnDialogueStarted?.Invoke();
            runner.StartDialogue(npc.yarnRootNode);
        }

        private void OnNodeStart(string nodeName)
        {
            Debug.Log($"Yarn node started: {nodeName}");
        }

        private void OnDialogueComplete()
        {
            OnDialogueEnded?.Invoke();
        }

        private void HandleSayCommand(string[] parameters)
        {
            if (parameters.Length < 2) return;

            string speaker = parameters[0];
            string line = parameters[1];

            var npc = npcDatabase.GetById(speaker.ToLower());
            Sprite portrait = npc != null ? npc.portrait : null;

            OnDialogueLineStarted?.Invoke(this, new DialogueLineEventArgs(speaker, line, portrait));
        }

        public int GetNpcProgress(string npcId)
        {
            if (variableStorage.TryGetValue($"${npcId}_progress", out float value))
                return Mathf.RoundToInt(value);
            return 0;
        }
    }
}
