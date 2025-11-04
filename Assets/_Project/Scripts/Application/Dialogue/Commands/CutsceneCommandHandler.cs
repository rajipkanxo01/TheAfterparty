using System.Threading.Tasks;
using _Project.Scripts.Presentation.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class CutsceneCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "cutscene";

        public async Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogWarning("CutsceneCommandHandler: Missing parameters");
                return;
            }

            var cutscene = Object.FindFirstObjectByType<FirstMemoryCutscene>();
            if (cutscene == null)
            {
                Debug.LogWarning("CutsceneCommandHandler: No FirstMemoryCutscene found in scene.");
                return;
            }

            string action = parameters[0].ToLower();

            switch (action)
            {
                case "next":
                    cutscene.NextAnimation();
                    await WaitForCutsceneToFinish(cutscene);
                    break;
                default:
                    Debug.LogWarning($"CutsceneCommandHandler: Unknown parameter '{action}'");
                    break;
            }
        }

        private async Task WaitForCutsceneToFinish(FirstMemoryCutscene cutscene)
        {
            while (cutscene.IsPlaying)
            {
                await Task.Yield();
            }
        }
    }
}