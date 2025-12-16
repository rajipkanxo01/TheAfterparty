using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private string sceneName;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                try
                {
                    SceneManager.LoadScene(sceneName);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load scene '{sceneName}': {e.Message}");
                }
            }
        }
    }
}