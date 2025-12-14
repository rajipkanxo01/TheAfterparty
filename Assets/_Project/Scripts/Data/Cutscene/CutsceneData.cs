using System.Collections.Generic;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Data.Cutscene
{

    [CreateAssetMenu(fileName = "New Cutscene", menuName = "Game/Cutscene/Cutscene", order = 0)]
    public class CutsceneData : ScriptableObject
    {
        [Header("Cutscene Info")]
        [Tooltip("Unique identifier for this cutscene")]
        public string cutsceneId;
        
        [Tooltip("Display name for this cutscene")]
        public string cutsceneName;
        
        [Header("Actions")]
        [Tooltip("List of actions to execute in this cutscene")]
        public List<ActionBaseData> actions = new();
        
        [Header("Playback Settings")]
        [Tooltip("If true, the cutscene will play automatically when triggered")]
        public bool playAutomatically = true;
        
        [Tooltip("If true, player input will be disabled during cutscene")]
        public bool disablePlayerInput = true;
        
        [Tooltip("If true, this cutscene can only be played once")]
        public bool playOnce = false;
    }
}