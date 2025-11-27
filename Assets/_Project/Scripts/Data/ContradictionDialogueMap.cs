using System;
using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(menuName = "Game/ContradictionDialogueMap")]
    public class ContradictionDialogueMap : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string observationId;
            public string yarnNodeName;
        }

        public Entry[] entries;

        public string GetNode(string observationId)
        {
            foreach (var e in entries)
            {
                if (e.observationId == observationId)
                {
                    return e.yarnNodeName;
                }
            }

            return "MogensContradiction_Generic";
        }
    }
}