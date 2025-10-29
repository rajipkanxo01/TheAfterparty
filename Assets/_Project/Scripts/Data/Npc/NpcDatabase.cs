using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Data.Npc
{
    [CreateAssetMenu(fileName = "NpcDatabase", menuName = "Game/NPC Database", order = 0)]
    public class NpcDatabase : ScriptableObject
    {
        [SerializeField] private List<NpcData> listOfNpc = new List<NpcData>();

        public NpcData GetById(string id)
        {
            return listOfNpc.FirstOrDefault(t => t != null && t.npcId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}