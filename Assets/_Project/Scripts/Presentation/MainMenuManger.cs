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
    }
}