using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Actions;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory.Actions;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryFragmentExecutor
    {
        private readonly List<IMemoryActionExecutor> _actionExecutors;
        private readonly MemoryActionContext _context;
        private readonly PlayerProfile _playerProfile;
        private readonly GameStateService _gameStateService;

        public MemoryFragmentExecutor(MemoryActionContext context)
        {
            _context = context;

            _actionExecutors = new List<IMemoryActionExecutor>
            {
                new DialogueActionExecutor(),
                new MoveActionExecutor()
            };

            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _gameStateService = ServiceLocater.GetService<GameStateService>();

            FragmentEvents.OnPlayFragmentStarted += HandlePlayFragmentStarted;
        }

        private void HandlePlayFragmentStarted(FragmentData fragment)
        {
            Debug.Log($"MemoryFragmentExecutor: Starting fragment {fragment.fragmentId}");
            _ = PlayFragmentAsync(fragment);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public async Task PlayFragmentAsync(FragmentData fragment)
        {
            var actions = SelectActions(fragment);

            foreach (var action in actions)
            {
                var executor = _actionExecutors.FirstOrDefault(x => x.CanExecute(action));
                if (executor != null)
                    await executor.ExecuteAsync(action, _context);
            }

            Debug.Log($"MemoryFragmentExecutor: Finished fragment {fragment.fragmentId}");
            _gameStateService.SetState(GameState.Normal);

            FragmentEvents.RaiseFragmentCompleted(fragment);
            FragmentEvents.RaiseSetActiveFragmentPoint(null);
        }

        private List<ActionBaseData> SelectActions(FragmentData fragment)
        {
            bool repaired = _playerProfile.HasRepairedFragment(fragment.fragmentId);

            if (repaired)
            {
                return fragment.realMemoryActions;
            };
            
            if (fragment.isCorrupted && fragment.HasCorruptedVersion)
            {
                return fragment.corruptedMemoryActions;
            }

            return fragment.realMemoryActions;
        }
        
        public void Dispose()
        {
            FragmentEvents.OnPlayFragmentStarted -= HandlePlayFragmentStarted;
        }
    }
}
