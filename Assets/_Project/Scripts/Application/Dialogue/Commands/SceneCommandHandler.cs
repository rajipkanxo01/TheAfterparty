using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class SceneCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "scene";

        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogWarning("SceneCommandHandler: Missing parameters. Usage: <<scene load SceneName>>");
                return;
            }

            string action = parameters[0].ToLower();

            if (action == "load" && parameters.Length > 1)
            {
                string sceneName = parameters[1];
                
                var dialogue = ServiceLocater.GetService<DialogueController>();
                if (dialogue != null)
                {
                    dialogue.EnableAutoMode(false);
                    Debug.Log("SceneCommandHandler: Auto mode disabled before loading scene.");
                }
                
                Debug.Log($"SceneCommandHandler: Loading scene '{sceneName}'...");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning($"SceneCommandHandler: Unknown command syntax: {string.Join(" ", parameters)}");
            }

            await Task.Yield();
        }
    }
}