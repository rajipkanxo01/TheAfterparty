using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Presentation.Npc;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory.Services
{
    public class MemoryNpcMoveService : MonoBehaviour, IMemoryNpcMoveService
    {
        [Tooltip("Parent object under which all npcs will be organized in the hierarchy.")] 
        [SerializeField] private Transform npcsParent;

        private void Awake()
        {
            ServiceLocater.RegisterService<IMemoryNpcMoveService>(this);
        }

        public async Task MoveAlongPathAsync(string npcId, IReadOnlyList<Vector3> positions, float speed)
        {
            if (string.IsNullOrEmpty(npcId))
            {
                Debug.Log("MemoryActorService: Empty NPC ID.");
                return;
            }

            if (positions == null || positions.Count == 0)
            {
                Debug.Log($"MemoryActorService: No positions provided for NPC '{npcId}'.");
                return;
            }

            if (npcsParent == null)
            {
                Debug.LogError("MemoryActorService: NPCs Parent is not assigned!");
                return;
            }

            var npcController = FindNpcController(npcId);
            if (npcController == null)
            {
                Debug.LogError($"MemoryActorService: NPC with ID '{npcId}' not found under NPCs Parent.");
                return;
            }
            
            await npcController.MoveAlongPathAsync(positions, speed);
        }

        private NpcController FindNpcController(string npcId)
        {
            foreach (Transform child in npcsParent)
            {
                var npcController = child.GetComponent<NpcController>();
                if (npcController != null && npcController.NpcId.Equals(npcId, StringComparison.OrdinalIgnoreCase))
                {
                    return npcController;
                }
            }

            return null;
        }
    }
}
