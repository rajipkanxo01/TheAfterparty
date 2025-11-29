using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    [CreateAssetMenu(fileName = "BezierMoveAction", menuName = "Game/Memory/Actions/BezierMove", order = 1)]
    public class BezierMoveActionBaseData : ActionBaseData
    {
        public string npcId;
        public float speed = 2f;
        public bool isLoop = false;

        [Tooltip("Vector3 positions in world space that the NPC will move to in order.")]
        public BezierPath path;

        public override string GetNpcId() => npcId;
    }
}