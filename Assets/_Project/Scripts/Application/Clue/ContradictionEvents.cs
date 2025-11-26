using System;

namespace _Project.Scripts.Application.Clue
{
    public class ContradictionEvents
    {
        // when a contradiction is found
        public static event Action<string, string> OnContradictionFound;
        public static void RaiseContradictionFound(string memoryId, string observationId) => OnContradictionFound?.Invoke(memoryId, observationId);
    }
}