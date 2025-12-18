using UnityEngine;

namespace _Project.Scripts.Presentation
{
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
            Debug.Log("Quitting game...");
            UnityEngine.Application.Quit(0);
        }
    }
}