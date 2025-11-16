using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Services;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryExecutorController : MonoBehaviour
    {
        private MemoryFragmentExecutor _executor;

        private void Start()
        {
            var npc = ServiceLocater.GetService<IMemoryNpcMoveService>();
            var dialogue = ServiceLocater.GetService<IMemoryDialogueService>();
            var context = new MemoryActionContext(dialogue, npc);

            _executor = new MemoryFragmentExecutor(context);
            Debug.Log("MemoryExecutorController: Created executor");
        }

        private void OnDestroy()
        {
            _executor?.Dispose();
            Debug.Log("MemoryExecutorController: Destroyed executor");
        }
    }
}