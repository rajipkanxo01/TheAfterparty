using System;
using System.Collections.Generic;
using _Project.Scripts.Data.Npc;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueManager
    {
        private Queue<DialogueLine> _dialogueLines;
        private DialogueLine _currentDialogueLine;
        
        public event Action<DialogueLine> OnDialogueLineStarted;
        public event Action OnDialogueEnded;

        public void StartDialogue(NpcData npc)
        {
            if (npc is null || npc.dialogueLines is null || npc.dialogueLines.Count is 0)
            {
                Debug.LogWarning($"[DialogueManager] Dialogue could not start: NPC '{npc?.npcId ?? "Unknown"}' has no dialogue lines assigned.");
                OnDialogueEnded?.Invoke();
                return;
            }
            
            _dialogueLines = new Queue<DialogueLine>(npc.dialogueLines);
            Debug.Log($"[DialogueManager] Dialogue line count for {npc.name} NPC: {npc.dialogueLines.Count}");
            ShowNextDialogueLine();
        }

        public void ShowNextDialogueLine()
        {
            if (_dialogueLines.Count == 0)
            {
                OnDialogueEnded?.Invoke();
                return;
            }
            
            _currentDialogueLine = _dialogueLines.Dequeue();
            Debug.Log($"[DialogueManager] Showing dialogue: {_currentDialogueLine.text}");
            OnDialogueLineStarted?.Invoke(_currentDialogueLine);
        }
    }
}