using System.Threading.Tasks;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Player;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class NotesCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "note";
        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length < 3)
            {
                UnityEngine.Debug.LogError("ContradictionCommandHandler: Invalid format for 'contradiction' command. Expected format: note <verify/contradiction> <memoryId> <observationId>");
                return;
            }
            
            string state = parameters[0];
            string memoryId = parameters[1];
            string observationId = parameters[2];

            ObservationState obsState = state.ToLower() switch
            {
                "verify" => ObservationState.Verified,
                "contradiction" => ObservationState.Contradicted,
                _ => ObservationState.Unknown
            };

            NotesEvent.RaiseNotesFound(obsState, memoryId, observationId);
        }
    }
}