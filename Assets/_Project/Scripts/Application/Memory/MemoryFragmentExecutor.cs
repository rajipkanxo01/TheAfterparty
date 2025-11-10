using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Actions;
using _Project.Scripts.Data.Memory.Actions;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryFragmentExecutor
    {
        private readonly List<IMemoryActionExecutor> _actionExecutors;
        private readonly MemoryActionContext _context;

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
            
            // register executors
            ServiceLocater.RegisterService<IMemoryActionExecutor>(dialogueActionExecutor);
            ServiceLocater.RegisterService<IMemoryActionExecutor>(moveActionExecutor);
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

            var orderedActions = actions.Where(a => a != null).OrderBy(a => a.order).ToList();
            
            foreach (var action in orderedActions)
            {
                foreach (var executor in _actionExecutors.Where(executor => executor.CanExecute(action)))
                {
                    await executor.ExecuteAsync(action, _context);
                    break; 
                }
            }
        }

        private List<ActionBaseData> SelectActions(FragmentData fragmentData)
        {
            // case 1: fragment not corrupted. always real
            if (!fragmentData.isCorrupted)
            {
                return fragmentData.realMemoryActions;
            }

            // case 2: fragment corrupted but repaired. show real
            if (fragmentData.isCorrupted && fragmentData.isRepaired)
            {
                return fragmentData.realMemoryActions;
            }
            
            // case 3: fragment corrupted and not repaired. show corrupted
            if (fragmentData.HasCorruptedVersion)
            {
                return fragmentData.corruptedMemoryActions;
            }
            
            // default: show real
            return fragmentData.realMemoryActions;
        }
    }
}