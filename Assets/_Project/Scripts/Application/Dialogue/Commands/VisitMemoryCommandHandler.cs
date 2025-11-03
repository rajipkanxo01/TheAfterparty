using System.Threading.Tasks;
using _Project.Scripts.Application.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class VisitMemoryCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "VisitMemory";

        public Task ExecuteAsync(string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogWarning("VisitMemoryCommandHandler: Missing memoryId parameter.");
                return Task.CompletedTask;
            }

            string memoryId = parameters[0];
            
            Debug.Log($"VisitMemoryCommandHandler: Visiting memory '{memoryId}'");
            MemoryEvents.RaiseVisitMemory(memoryId);
            
            return Task.CompletedTask;
        }
    }
}