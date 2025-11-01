using UnityEngine;

namespace _Project.Scripts.Data.Clues
{
    [CreateAssetMenu(fileName = "SniffConfig", menuName = "Game/Sniff Config", order = 0)]
    public class SniffConfig : ScriptableObject
    {
        [Range(1f, 15f)] [SerializeField] private float sniffRadius = 5f;
        [SerializeField] private float sniffCooldown = 3f;
        [SerializeField] private float sniffCooldownToastDuration = 0.5f;
        
        public float SniffRadius => sniffRadius;
        public float SniffCooldown => sniffCooldown;
        public float SniffCooldownToastDuration => sniffCooldownToastDuration;
    }
}