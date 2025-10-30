using System;
using _Project.Scripts.Data.Clues;

namespace _Project.Scripts.Application.Clue
{
    public static class ClueEvents
    {
        public static event Action<string> OnClueExamined;
        public static event Action<ClueData> OnClueDiscovered;
        
        public static void RaiseExamined(string clueNode) => OnClueExamined?.Invoke(clueNode);
        public static void RaiseDiscovered(ClueData clue) => OnClueDiscovered?.Invoke(clue);
    }
}