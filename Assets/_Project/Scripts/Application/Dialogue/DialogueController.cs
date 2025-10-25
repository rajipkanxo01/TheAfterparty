using System;
using _Project.Scripts.Data.Npc;
using UnityEngine;
using Yarn.Unity;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private NpcDatabase npcDatabase;
        
        // events presentation layer can subscribe to
        public event EventHandler<DialogueLineEventArgs> OnDialogueLineStarted;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;


        private void Awake()
        {
            if (dialogueRunner == null)
            {
                Debug.LogError("DialogueController: DialogueRunner reference missing.");
                return;
            }
            
            dialogueRunner.onNodeStart?.AddListener(HandleNodeStart);
            dialogueRunner.onNodeComplete?.AddListener(HandleNodeComplete);
            dialogueRunner.onDialogueComplete?.AddListener(HandleDialogueComplete);

            dialogueRunner.DialoguePresenters = Array.Empty<DialoguePresenterBase>();
            dialogueRunner.AddCommandHandler("say", new Action<string[]>(HandleSayCommand));
        }

        public void StartDialogueWithNpc(string npcId)
        {
            NpcData npcData = npcDatabase.GetById(npcId);
            if (npcData == null)
            {
                Debug.LogError($"DialogueController: NPC with ID {npcId} not found in database.");
                return;
            }

            OnDialogueStarted?.Invoke();
            dialogueRunner.StartDialogue(npcData.yarnStartNode);
        }
        
        private void HandleNodeStart(string nodeName)
        {
            Debug.Log($"Dialogue node started: {nodeName}");
        }

        private void HandleNodeComplete(string nodeName)
        {
            Debug.Log($"Dialogue node completed: {nodeName}");
        }

        private void HandleDialogueComplete()
        {
            OnDialogueEnded?.Invoke();
        }
        
        private void HandleSayCommand(string[] parameters)
        {
            if (parameters.Length < 2)
            {
                Debug.LogError("DialogueController: 'say' command requires at least 2 parameters (speaker and line).");
                return;
            }

            string speakerName = parameters[0];
            string line = parameters[1];
            
            var npc = npcDatabase.GetById(speakerName);
            Sprite portrait = npc != null ? npc.portrait : null;

            OnDialogueLineStarted?.Invoke(this, new DialogueLineEventArgs(speakerName, line, portrait));
        }
    }
}