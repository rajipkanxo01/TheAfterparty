using _Project.Scripts.Application.Memory;
using _Project.Scripts.Data.Npc;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue
{
    public class NpcDialogueHandler
    {
        private readonly DialogueController _controller;
        private readonly NpcDatabase _npcDatabase;
        private NpcData _currentNpc;
        
        public NpcData CurrentNpc => _currentNpc;
        
        public NpcDialogueHandler(DialogueController controller, NpcDatabase npcDatabase)
        {
            _controller = controller;
            _npcDatabase = npcDatabase;
        }
        
        
        // todo: maybe this is not needed
        
        /*public void StartNpcConversation(string npcId)
        {
            if (string.IsNullOrEmpty(npcId))
            {
                Debug.LogWarning("NpcDialogueHandler: NPC ID is null or empty.");
                return;
            }

            var npc = _npcDatabase.GetById(npcId);
            if (npc == null)
            {
                Debug.LogWarning($"NpcDialogueHandler: NPC '{npcId}' not found in database.");
                return;
            }

            _currentNpc = npc;
            Debug.Log($"NpcDialogueHandler: Starting conversation with '{npc.npcName}'");

            _controller.StartDialogue(npc.yarnRootNode, DialogueType.NpcConversation);
        }*/

        public void HandleConversationEnd()
        {
            if (_currentNpc == null)
                return;

            Debug.Log($"NpcDialogueHandler: Conversation ended with '{_currentNpc.npcName}'");

            // todo: ???
            /*if (!string.IsNullOrEmpty(_currentNpc.memorySceneName))
            {
                MemoryEvents.RaiseMemoryUnlocked(_currentNpc.memorySceneName);
            }*/

            _currentNpc = null;
        }
    }
}