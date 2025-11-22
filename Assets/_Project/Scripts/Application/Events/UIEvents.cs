using System;

namespace _Project.Scripts.Application.Events
{
    public static class UIEvents
    {
        // when journal is opened
        public static event Action OnJournalOpen;
        public static void RaiseJournalOpen() => OnJournalOpen?.Invoke();
        
        // when journal is closed
        public static event Action OnJournalClose;
        public static void RaiseJournalClose() => OnJournalClose?.Invoke();
    }
}