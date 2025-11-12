using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class CameraRestacker : MonoBehaviour
    {
        [SerializeField] private Camera baseCamera;
        [SerializeField] private Camera playerCamera;

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
            var baseData = baseCamera.GetComponent<UniversalAdditionalCameraData>();
            baseData.cameraStack.Clear();

            if (playerCamera != null)
            {
                baseData.cameraStack.Add(playerCamera);
            };
        }
    }
}