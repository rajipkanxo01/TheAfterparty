using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    public class PlayerProfile
    {
        public string DisplayName { get; set; }
        public string PlayerId { get; set; }
        public Sprite Portrait { get; set; }
        
        public PlayerProfile(string displayName, string playerId, Sprite portrait)
        {
            DisplayName = displayName;
            PlayerId = playerId;
            Portrait = portrait;
        }
    }
}