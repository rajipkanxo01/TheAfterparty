using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [System.Serializable]
    public class BezierPath 
    {
        public List<Vector3> controlPoints = new List<Vector3>() { };
        public Vector3 endPoint;
    }


    [CreateAssetMenu(fileName = "BezierMoveAction", menuName = "Game/Memory/Actions/BezierMove", order = 1)]
    public class BezierMoveActionBaseData : ActionBaseData
    {
        public string npcId;
        public float speed = 2f;
        public bool isLoop = false;
        
        [Tooltip("Vector3 positions in world space that the NPC will move to in order. " +
                 "Within each List<Vector3>, the last one will be the point the NPC ends up at.")]
        public BezierPath[] paths = new BezierPath[] { };
        
        public override string GetNpcId() => npcId;
    }
}