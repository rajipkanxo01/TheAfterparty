using UnityEngine;

namespace _Project.Scripts.Data.Npc
{
    public class NpcRepository
    {
        public NpcData GetNpc(string npcId)
        {
            Debug.Log($"[NpcRepository] GetNpc: npcId={npcId}");
            
            var path = $"Npc/{npcId}";
            // todo: needed to search alternative for this Resources.Load in future for better performance and scalability
            var npc = Resources.Load<NpcData>(path);

            Debug.Log(npc != null ? $"Loaded NpcData at {path}" : $"Failed to load NpcData at {path}");

            return npc;
        }
    }
}