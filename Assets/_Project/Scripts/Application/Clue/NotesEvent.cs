using System;
using _Project.Scripts.Application.Player;

namespace _Project.Scripts.Application.Clue
{
    public static class NotesEvent
    {
        // when notes is found
        public static event Action<ObservationState, string, string> OnNotesFound;
        public static void RaiseNotesFound(ObservationState state, string memoryId, string observationId) => OnNotesFound?.Invoke(state, memoryId, observationId);
    }
}