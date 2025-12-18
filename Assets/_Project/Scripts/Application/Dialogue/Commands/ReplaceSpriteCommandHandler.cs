using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Presentation.Memory.Services;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class ReplaceSpriteCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "replace_sprite";

        public Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogWarning("ReplaceSpriteCommandHandler: No target name provided to <<replace_sprite>>");
                return Task.CompletedTask;
            }

            string targetName = parameters[0];
            Debug.Log($"ReplaceSpriteCommandHandler: Replacing sprite for '{targetName}'");

            var spriteManager = ServiceLocater.GetService<MemorySpriteManager>();
            
            if (spriteManager == null)
            {
                Debug.LogWarning("ReplaceSpriteCommandHandler: MemorySpriteManager not found in scene. " +
                                 "Make sure MemorySpriteManager component is added to the memory scene.");
                return Task.CompletedTask;
            }

            spriteManager.ReplaceSprite(targetName);
            
            return Task.CompletedTask;
        }
    }
}