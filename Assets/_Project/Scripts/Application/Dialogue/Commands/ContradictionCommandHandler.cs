using System.Threading.Tasks;
using _Project.Scripts.Application.Clue;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class ContradictionCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "contradiction";
        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                UnityEngine.Debug.LogError("ContradictionCommandHandler: Invalid format for 'contradiction' command. Expected format: contradiction <found> <observationId>");
                return;
            }
            
            string mode = parameters[0];
            string observationId = parameters[1];
            
            if (mode.Equals("found"))
            {
                // Raise event for contradiction found
                ContradictionEvents.RaiseContradictionFound(observationId);
            }
        }
    }
}