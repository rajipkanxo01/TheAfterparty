using System.Threading.Tasks;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Actions
{
    public class DialogueActionExecutor : IMemoryActionExecutor
    {
        public bool CanExecute(ActionBaseData actionData)
        {
            return actionData is DialogueActionData;
        }

        public async Task ExecuteAsync(ActionBaseData actionData, MemoryActionContext context)
        {
            var data = actionData as DialogueActionData;
            if (data == null || string.IsNullOrEmpty(data.yarnNodeName))
            {
                Debug.LogError("DialogueActionExecutor: Invalid DialogueActionData or missing yarnNodeName.");
                return;
            }

            var dialogueService = context.DialogueService;
            if (dialogueService == null)
            {
                Debug.LogError("DialogueActionExecutor: DialogueService is not available in the context.");
                return;
            }
            
            
            Debug.Log("DialogueActionExecutor: Playing dialogue node '" + data.yarnNodeName + "'.");
            await dialogueService.PlayNodeAsync(data.yarnNodeName);
        }
    }
}