using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcAnchor : MonoBehaviour
    {
        [SerializeField] private string npcId;
        [SerializeField] private Transform headPoint;
        
        public string NpcId => npcId;
        public Transform HeadPoint => headPoint;
    }
}