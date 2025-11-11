using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Application.Memory.Services;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Actions
{
    public class MoveActionExecutor : IMemoryActionExecutor
    {
        public bool CanExecute(ActionBaseData actionData)
        {
            return actionData as MoveActionBaseData;
        }

        public async Task ExecuteAsync(ActionBaseData actionData, MemoryActionContext context)
        {
            var data = actionData as MoveActionBaseData;
            if (data == null || string.IsNullOrEmpty(data.actorId))
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

            List<Vector3> positions = GetPathPositions(data.pathPointNames);
            if (positions.Count == 0)
            {
                Debug.LogWarning("MoveActionExecutor: No valid path positions found.");
                return;
            }

            await npcMoveService.MoveAlongPathAsync(data.actorId, positions, data.speed);
        }

        private List<Vector3> GetPathPositions(List<string> names)
        {
            var result = new List<Vector3>();

            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var obj = GameObject.Find(name);
                if (obj != null)
                {
                    result.Add(obj.transform.position);
                }
                else
                {
                    Debug.LogWarning($"MoveActionExecutor: No GameObject found with name '{name}'.");
                }
            }

            return result;
        }
    }
}