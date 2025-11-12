using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation
{
    public class MainMenuManger : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad = "MainScene";
        
        public void StartGame()
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        public void PauseGame()
        {
            if(GameObject.Find("PauseMenu")?.TryGetComponent<PauseMenu>(out PauseMenu pm) ?? false)
            {
                pm.SetPause(true);
            }
        }
    }
}