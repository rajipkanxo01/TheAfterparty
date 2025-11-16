using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [CreateAssetMenu(fileName = "MoveAction", menuName = "Game/Memory/Actions/Move", order = 0)]
    public class MoveActionBaseData : ActionBaseData
    {
        public string npcId;
        public float speed = 2f;
        public bool isLoop = false;
        
        [Tooltip("Names of the path points this actor will follow (in order).")]
        public List<string> pathPointNames = new List<string>();
        
        public override string GetNpcId() => npcId;
    }
}