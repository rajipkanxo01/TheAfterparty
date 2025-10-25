using _Project.Scripts.Data.Npc;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueService
    {
        private readonly NpcRepository _npcRepository = new();
        private readonly DialogueManager _dialogueManager = new();

        public DialogueManager DialogueManager => _dialogueManager;

        public void StartDialogue(string npcId)
        {
            NpcData npc = _npcRepository.GetNpc(npcId);
            
            Debug.Log($"[DialogueService] Starting dialogue with NPC: {npc.npcName}");
            _dialogueManager.StartDialogue(npc);
        }
    }
}