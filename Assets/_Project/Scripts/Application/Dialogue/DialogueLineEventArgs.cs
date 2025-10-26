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
}