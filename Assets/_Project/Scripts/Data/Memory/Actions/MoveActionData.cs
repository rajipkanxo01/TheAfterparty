using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [CreateAssetMenu(fileName = "MoveAction", menuName = "Game/Memory/Actions/Move", order = 0)]
    public class MoveActionBaseData : ActionBaseData
    {
        public string npcId;
        public float speed = 2f;
        public bool isLoop = false;
        
        [Tooltip("Vector3 positions in world space that the NPC will move to in order.")]
        public Vector3[] paths = new Vector3[] { };
        
        public override string GetNpcId() => npcId;
    }
}