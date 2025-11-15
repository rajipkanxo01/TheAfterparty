using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [CreateAssetMenu(fileName = "New Memory Echo", menuName = "Game/Memory/Memory Echo", order = 0)]
    public class MemoryEchoData : ScriptableObject
    {
        public string echoId;
        public string fragmentId;
    }
}