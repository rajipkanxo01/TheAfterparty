using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Actions;
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

        public MemoryFragmentExecutor(MemoryActionContext context)
        {
            _context = context;

            var dialogueActionExecutor = new DialogueActionExecutor();
            var moveActionExecutor = new MoveActionExecutor();
            
            _actionExecutors = new List<IMemoryActionExecutor>
            {
                dialogueActionExecutor,
                moveActionExecutor
            };

            ServiceLocater.RegisterService<IMemoryActionExecutor>(dialogueActionExecutor);
            ServiceLocater.RegisterService<IMemoryActionExecutor>(moveActionExecutor);
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        public async Task PlayFragmentAsync(FragmentData fragmentData)
        {
            if (fragmentData == null || fragmentData.realMemoryActions == null)
            {
                Debug.LogWarning("MemoryFragmentExecutor: FragmentData or its actions are null.");
                return;
            }

            var actions = SelectActions(fragmentData);
            if (actions == null || actions.Count == 0)
            {
                Debug.LogWarning($"MemoryFragmentExecutor: No actions found for fragment {fragmentData.fragmentId}");
                return;
            }

            // Order is still your primary control for rough timeline
            var orderedActions = actions
                .Where(a => a != null)
                .OrderBy(a => a.order)
                .ToList();

            var parallelAndDelayedTasks = new List<Task>();
            var fragmentStartTime = Time.realtimeSinceStartup;

            // Tracks the "chain" of sequential actions so they run one after another
            Task lastSequentialTask = Task.CompletedTask;

            foreach (var action in orderedActions)
            {
                var executor = _actionExecutors.FirstOrDefault(ex => ex.CanExecute(action));
                if (executor == null)
                {
                    Debug.LogWarning($"MemoryFragmentExecutor: No executor found for action {action.name}");
                    continue;
                }

                switch (action.playMode)
                {
                    case ActionPlayMode.Sequential:
                    {
                        lastSequentialTask = RunSequentialAsync(
                            lastSequentialTask,
                            executor,
                            action,
                            fragmentStartTime
                        );
                        break;
                    }

                    case ActionPlayMode.Parallel:
                    {
                        var task = executor.ExecuteAsync(action, _context);
                        parallelAndDelayedTasks.Add(task);
                        break;
                    }

                    case ActionPlayMode.DelayedFromFragmentStart:
                    {
                        var task = RunDelayedFromStartAsync(
                            executor,
                            action,
                            fragmentStartTime
                        );
                        parallelAndDelayedTasks.Add(task);
                        break;
                    }
                }
            }

            // Wait for sequential chain to finish
            await lastSequentialTask;

            // Wait for all parallel + delayed actions to finish
            if (parallelAndDelayedTasks.Count > 0)
            {
                await Task.WhenAll(parallelAndDelayedTasks);
            }
        }

        private async Task RunSequentialAsync(
            Task previousSequential,
            IMemoryActionExecutor executor,
            ActionBaseData action,
            float fragmentStartTime)
        {
            // Wait for previous sequential action
            await previousSequential;

            if (action.startDelaySeconds > 0f)
            {
                float targetTime = fragmentStartTime + action.startDelaySeconds;
                float now = Time.realtimeSinceStartup;
                float waitSeconds = targetTime - now;

                if (waitSeconds > 0f)
                {
                    await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
                }
            }

            await executor.ExecuteAsync(action, _context);
        }

        private async Task RunDelayedFromStartAsync(
            IMemoryActionExecutor executor,
            ActionBaseData action,
            float fragmentStartTime)
        {
            float delay = Mathf.Max(0f, action.startDelaySeconds);
            float targetTime = fragmentStartTime + delay;

            while (true)
            {
                float now = Time.realtimeSinceStartup;
                float remaining = targetTime - now;
                if (remaining <= 0f)
                    break;

                var step = Mathf.Min(remaining, 0.1f);
                await Task.Delay(TimeSpan.FromSeconds(step));
            }

            await executor.ExecuteAsync(action, _context);
        }

        private List<ActionBaseData> SelectActions(FragmentData fragmentData)
        {
            if (fragmentData == null)
                return null;

            bool repaired = _playerProfile.HasRepairedFragment(fragmentData.fragmentId);

            Debug.Log($"MemoryFragmentExecutor: Fragment '{fragmentData.fragmentId}' repaired={repaired}, assetCorrupted={fragmentData.isCorrupted}");

            if (repaired)
            {
                return fragmentData.realMemoryActions;
            }

            if (fragmentData.isCorrupted && fragmentData.HasCorruptedVersion)
            {
                return fragmentData.corruptedMemoryActions;
            }

            return fragmentData.realMemoryActions;
        }


    }
}
