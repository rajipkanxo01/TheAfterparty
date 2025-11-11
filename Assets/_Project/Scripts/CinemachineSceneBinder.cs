using _Project.Scripts.Presentation.Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class CinemachineSceneBinder : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player == null)
            {
                Debug.LogWarning($"CinemachineSceneBinder: No PlayerController found in scene '{scene.name}'.");
                return;
            }

            if (virtualCamera == null)
            {
                virtualCamera = FindFirstObjectByType<CinemachineCamera>();
            }

            if (virtualCamera != null)
            {
                virtualCamera.Follow = player.transform;
                virtualCamera.LookAt = player.transform;
            }
        }
    }
}