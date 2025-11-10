using _Project.Scripts.Application.Memory.Services;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryActionContext
    {
        public IMemoryDialogueService DialogueService { get; private set; }
        public IMemoryActorService ActorService { get; private set; }

        public MemoryActionContext(IMemoryDialogueService dialogueService, IMemoryActorService actorService)
        {
            DialogueService = dialogueService;
            ActorService = actorService;
        }
    }
}