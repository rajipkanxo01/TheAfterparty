using UnityEngine;

namespace _Project.Scripts.Data.Player
{
    [CreateAssetMenu(menuName = "Game/Player Profile", fileName = "PlayerProfile")]
    public class PlayerProfileSo : ScriptableObject
    {
        public string displayName = "Detective’s Cat";
        public Sprite portrait;
        public string playerId = "player";
    }
}