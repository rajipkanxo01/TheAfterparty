using UnityEngine;

namespace _Project.Scripts
{
    public class DoNotDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}