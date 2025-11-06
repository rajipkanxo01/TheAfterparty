using System.Threading.Tasks;
using _Project.Scripts.Application.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class MemoryCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "memory";

        public MemoryCommandHandler()
        {
        }

        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogError("MemoryCommandHandler: Invalid format for 'memory' command. Expected format: memory <unlock|visit> <sceneId>");
                return;
            }
            
            string mode = parameters[0];
            string sceneId = parameters[1];

            if (mode.Equals("unlock"))
            {
                MemoryEvents.RaiseMemoryUnlocked(sceneId);
            }
            else if (mode.Equals("visit"))
            {
                MemoryEvents.RaiseVisitMemory(sceneId);
            }

        }
    }
}