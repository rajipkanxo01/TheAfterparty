using System;

namespace _Project.Scripts.Application.Dialogue
{
    public static class DialogueEvents
    {
        public static event EventHandler<DialogueLineEventArgs> OnDialogueLineStarted;
        public static event Action OnDialogueStarted;
        public static event Action OnDialogueEnded;
        public static event Action OnDialogueContinued;
        
        public static void RaiseDialogueLineStarted(object sender, DialogueLineEventArgs args)
        {
            OnDialogueLineStarted?.Invoke(sender, args);
        }
        
        public static void RaiseDialogueStarted()
        {
            OnDialogueStarted?.Invoke();
        }
        
        public static void RaiseDialogueEnded()
        {
            OnDialogueEnded?.Invoke();
        }
        
        public static void RaiseDialogueContinued()
        {
            OnDialogueContinued?.Invoke();
        }
    }
}