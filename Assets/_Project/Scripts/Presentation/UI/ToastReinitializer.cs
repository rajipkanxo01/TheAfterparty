using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.UI
{
    public class ToastReinitializer : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var toast = GetComponent<ToastNotification>();
            if (toast != null)
            {
                var rect = GetComponent<RectTransform>();
                if (rect != null)
                {
                    toast.SendMessage("Awake"); 
                }
            }
        }
    }
}