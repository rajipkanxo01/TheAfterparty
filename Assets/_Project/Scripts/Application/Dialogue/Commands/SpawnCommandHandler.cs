using System.Threading.Tasks;
using _Project.Scripts.Application.Badge;
using _Project.Scripts.Application.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class BadgeCommandHandler : IDialogueCommandHandler
    {
        public string CommandName => "spawn_badge";

        public async Task ExecuteAsync(params string[] parameters)
        {
            MemoryEvents.OnMemoryTransitionEnd += OnTransitionEnd;

            // Wait until after memory transition
            async void OnTransitionEnd()
            {
                MemoryEvents.OnMemoryTransitionEnd -= OnTransitionEnd;
                Debug.Log("[BadgeManager] Raising spawn badge event (after transition).");
                
                // todo: delay a bit more to ensure everything is settled
                await Task.Yield();
                await Task.Yield();
                BadgeEvents.RaiseSpawnBadge();
            }
            
            await Task.CompletedTask;
        }
    }
}