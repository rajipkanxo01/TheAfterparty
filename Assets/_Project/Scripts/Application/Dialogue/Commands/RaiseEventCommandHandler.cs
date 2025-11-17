using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class RaiseEventCommandHandler : IDialogueCommandHandler
    {
        private readonly PlayerProfile _playerProfile = ServiceLocater.GetService<PlayerProfile>();

        public string CommandName => "raise_event";

        public Task ExecuteAsync(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogWarning("RaiseEventCommandHandler: No parameters passed to <<raise_event>>");
                return Task.CompletedTask;
            }

            string eventId = parameters[0];
            Debug.Log($"RaiseEventCommandHandler: Received event '{eventId}' from Yarn.");

            HandleEvent(eventId);
            return Task.CompletedTask;
        }

        private void HandleEvent(string eventId)
        {
            if (int.TryParse(eventId, out int code))
            {
                Debug.Log($"RaiseEventCommandHandler: Treating '{eventId}' as chest code.");
                
                string flagKey = $"ChestCode_{eventId}";
                _playerProfile.SetFlag(flagKey, true);

                Debug.Log($"RaiseEventCommandHandler: Saved flag '{flagKey}' = true");
                return;
            }
        }
    }
}