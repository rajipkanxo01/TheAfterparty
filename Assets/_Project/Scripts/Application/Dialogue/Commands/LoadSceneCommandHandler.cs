using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class LoadSceneCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "load_scene";

        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
            {
                Debug.LogError("LoadSceneCommandHandler: Invalid format for 'load_scene' command. Expected format: load_scene <sceneName>");
                return;
            }
            
            string sceneName = parameters[0];
            
            Debug.Log($"LoadSceneCommandHandler: Loading scene '{sceneName}'");
            
            // Add a small delay to ensure dialogue completes
            await Task.Delay(500);
            
            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
    }
}

