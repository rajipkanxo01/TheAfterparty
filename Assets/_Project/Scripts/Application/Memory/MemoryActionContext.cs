using _Project.Scripts.Application.Memory.Services;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryActionContext
    {
        public IMemoryDialogueService DialogueService { get; private set; }
        public IMemoryNpcMoveService NpcMoveService { get; private set; }

        public MemoryActionContext(IMemoryDialogueService dialogueService, IMemoryNpcMoveService npcMoveService)
        {
            DialogueService = dialogueService;
            NpcMoveService = npcMoveService;
        }
    }
}