using System;

namespace _Project.Scripts.Application.Clue
{
    public static class ContradictionEvents
    {
        // when a contradiction is found
        public static event Action<string> OnContradictionFound;
        public static void RaiseContradictionFound(string observationId) => OnContradictionFound?.Invoke(observationId);
    }
}