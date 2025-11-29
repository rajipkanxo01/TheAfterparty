using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Application.Utilities;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Actions
{
    public class MoveActionExecutor : IMemoryActionExecutor
    {
        public bool CanExecute(ActionBaseData actionData)
        {
            return actionData as BezierMoveActionBaseData;
        }

        public async Task ExecuteAsync(ActionBaseData actionData, MemoryActionContext context)
        {
            Debug.Log("MoveActionExecutor: Executing move action.");
            var data = actionData as BezierMoveActionBaseData;
            if (data == null || string.IsNullOrEmpty(data.npcId))
            {
                Debug.LogError("MoveActionExecutor: Invalid MoveActionBaseData or missing actorId.");
                return;
            }

            IMemoryNpcMoveService npcMoveService = context.NpcMoveService;
            if (npcMoveService == null)
            {
                Debug.LogError("MoveActionExecutor: NpcService is null.");
                return;
            }

            BezierPath path = data.path;
            if (path == null)
            {
                Debug.LogWarning("MoveActionExecutor: No valid path positions found.");
                return;
            }
            
            Debug.Log("MoveActionExecutor: Starting movement for actor " + data.npcId);
            await npcMoveService.MoveAlongPathAsync(data.npcId, path, data.speed);
        }
        
    }
}