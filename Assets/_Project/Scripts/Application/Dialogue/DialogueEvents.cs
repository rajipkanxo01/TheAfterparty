using System;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueLineEventArgs : EventArgs
    {
        public string SpeakerId;
        public string SpeakerName;
        public string LineText;
        public Sprite Portrait;
        
        public DialogueLineEventArgs(string speakerName, string lineText, Sprite portrait, string speakerId)
        {
            SpeakerName = speakerName;
            LineText = lineText;
            Portrait = portrait;
            SpeakerId = speakerId;
        }
    }
    
    public static class DialogueEvents
    {
        public static event EventHandler<DialogueLineEventArgs> OnDialogueLineStarted;
        public static event Action OnDialogueStarted;
        public static event Action OnDialogueEnded;
        public static event Action OnDialogueContinued;
        public static event Action OnDialogueContinueRequested;
        
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
        
        public static void RaiseDialogueContinueRequested()
        {
            Debug.Log("DialogueEvents: Raising OnDialogueContinueRequested event.");
            OnDialogueContinueRequested?.Invoke();
        }
    }
}