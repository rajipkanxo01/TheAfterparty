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
        
        // when tab index changes
        public static event Action<int> OnJournalTabChanged;
        public static void RaiseJournalTabChanged(int index) => OnJournalTabChanged?.Invoke(index);
        
        // when notes have changed
        public static event Action OnJournalNotesChanged;
        public static void RaiseJournalNotesChanged() => OnJournalNotesChanged?.Invoke();
    }
}