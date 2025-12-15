using _Project.Scripts.Presentation.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation
{
    public class MainMenuManger : MonoBehaviour
    {
        public void StartGame()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex+1);
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