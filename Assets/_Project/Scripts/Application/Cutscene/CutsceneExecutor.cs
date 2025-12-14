using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Cutscene.Events;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Actions;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Cutscene;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Application.Cutscene
{
    public class CutsceneExecutor
    {
        private readonly List<IMemoryActionExecutor> _actionExecutors;
        private readonly MemoryActionContext _context;
        private readonly GameStateService _gameStateService;
        private readonly PlayerProfile _playerProfile;
        private bool _isPlaying = false;

        public bool IsPlaying => _isPlaying;

        public CutsceneExecutor(MemoryActionContext context)
        {
            _context = context;
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();

            // Reuse existing action executors
            _actionExecutors = new List<IMemoryActionExecutor>
            {
                new DialogueActionExecutor(),
                new MoveActionExecutor()
            };

            CutsceneEvents.OnPlayCutsceneRequested += HandlePlayCutsceneRequested;
        }

        private void HandlePlayCutsceneRequested(string cutsceneId)
        {
            Debug.Log($"CutsceneExecutor: Received request to play cutscene {cutsceneId}");
            // Cutscene will be played by CutsceneManager, this is just for logging
        }
        
        public async Task PlayCutsceneAsync(CutsceneData cutscene)
        {
            if (cutscene == null)
            {
                Debug.LogError("CutsceneExecutor: Cannot play null cutscene");
                return;
            }

            // Check if cutscene should only play once
            if (cutscene.playOnce && _playerProfile.HasPlayedCutscene(cutscene.cutsceneId))
            {
                Debug.Log($"CutsceneExecutor: Cutscene {cutscene.cutsceneId} has already been played and is set to play once only");
                return;
            }

            _isPlaying = true;
            
            Debug.Log($"CutsceneExecutor: Starting cutscene {cutscene.cutsceneId}");
            
            // Set game state
            if (cutscene.disablePlayerInput)
            {
                _gameStateService.SetState(GameState.Dialogue);
            }
            
            CutsceneEvents.RaiseCutsceneStarted(cutscene);

            // Sort actions by order and execute sequentially
            var sortedActions = cutscene.actions.OrderBy(a => a.order).ToList();

            foreach (var action in sortedActions)
            {
                Debug.Log($"CutsceneExecutor: Executing action of type {action.GetType().Name}");
                
                var executor = _actionExecutors.FirstOrDefault(x => x.CanExecute(action));

                if (executor == null)
                {
                    Debug.LogWarning($"CutsceneExecutor: No executor found for action type {action.GetType().Name}");
                    continue;
                }

                if (action.startDelaySeconds > 0)
                {
                    await Task.Delay((int)(action.startDelaySeconds * 1000));
                }
                
                // Execute action and wait for completion
                await executor.ExecuteAsync(action, _context);
            }

            Debug.Log($"CutsceneExecutor: Completed cutscene {cutscene.cutsceneId}");
            
            if (cutscene.playOnce)
            {
                _playerProfile.MarkCutscenePlayed(cutscene.cutsceneId);
            }
            
            _gameStateService.SetState(GameState.Normal);
            
            CutsceneEvents.RaiseCutsceneCompleted(cutscene);
            
            _isPlaying = false;
        }


        public void Dispose()
        {
            CutsceneEvents.OnPlayCutsceneRequested -= HandlePlayCutsceneRequested;
        }
    }
}

