using UnityEngine;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Npc;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueSpeakerResolver
    {
        private readonly NpcDatabase _npcDatabase;

        public DialogueSpeakerResolver(NpcDatabase npcDatabase)
        {
            _npcDatabase = npcDatabase;
        }

        public (string name, Sprite portrait) Resolve(string speakerId, PlayerProfile playerProfile, DialogueType type)
        {
            if (string.IsNullOrWhiteSpace(speakerId))
            {
                return ("Unknown", null);
            }

            string id = speakerId.Trim().ToLowerInvariant();

            if (id == "cat")
            {
                return playerProfile != null ? (playerProfile.DisplayName, playerProfile.Portrait) : ("Player", null);
            }

            var npc = _npcDatabase?.GetById(speakerId);
            if (npc != null)
            {
                return (npc.npcName, npc.portrait);
            }

            return type == DialogueType.PlayerMonologue && playerProfile != null
                ? (playerProfile.DisplayName, playerProfile.Portrait)
                : (speakerId, null);
        }
    }
}