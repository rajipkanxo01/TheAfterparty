using System;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue
{
    public class DialogueLineEventArgs : EventArgs
    {
        public string SpeakerName;
        public string LineText;
        public Sprite Portrait;
        
        public DialogueLineEventArgs(string speakerName, string lineText, Sprite portrait)
        {
            SpeakerName = speakerName;
            LineText = lineText;
            Portrait = portrait;
        }
    }
}