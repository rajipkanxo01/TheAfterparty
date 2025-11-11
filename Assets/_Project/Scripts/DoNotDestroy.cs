using UnityEngine;

namespace _Project.Scripts
{
    public class DoNotDestroy : MonoBehaviour
    {
        private void Awake()
        {
            var existing = GameObject.Find(gameObject.name);
            if (existing != null && existing != gameObject)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}