using UnityEngine;

namespace _Project.Scripts.Data.Objects
{
    [CreateAssetMenu(menuName = "Game/Objects/Chest")]
    public class ChestData : ScriptableObject
    {
        public string chestId;
        public string correctCode;
    }
}