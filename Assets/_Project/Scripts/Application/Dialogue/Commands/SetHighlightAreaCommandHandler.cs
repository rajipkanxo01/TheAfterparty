using System.Threading.Tasks;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.MemoryPointer;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class SetHighlightAreaCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "set_active_point";

        public Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
            {
                Debug.LogWarning("SetHighlightAreaCommandHandler: Missing point ID for 'set_active_point'");
                return Task.CompletedTask;
            }

            string pointId = parameters[0];

            var point = MemoryImportantRegistry.GetPoint(pointId);
            if (point == null)
            {
                Debug.LogError($"SetHighlightAreaCommandHandler: No important point found with ID '{pointId}'");
                return Task.CompletedTask;
            }

            FragmentEvents.RaiseSetActiveFragmentPoint(point);

            return Task.CompletedTask;
        }
    }
}