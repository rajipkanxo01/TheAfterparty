using UnityEngine;

namespace _Project.Scripts.Data.Npc
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea] public string text;
    }
}