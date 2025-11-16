using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Echo
{
    public class MemoryEchoService
    {
        private readonly FragmentDatabase _fragmentDatabase;
        private readonly GameStateService _gameStateService;
        
        public MemoryEchoService()
        {
            FragmentEvents.OnPlayFragmentRequested += HandlePlayFragmentRequested;
        
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _fragmentDatabase = ServiceLocater.GetService<FragmentDatabase>();
        }

        private void HandlePlayFragmentRequested(string fragmentId)
        {
            var fragmentData = _fragmentDatabase.GetById(fragmentId);
            if (fragmentData == null)
            {
                Debug.LogError("MemoryEchoService: Fragment data not found for id: " + fragmentId);
                return;
            }
            
            FragmentEvents.RaiseFragmentActivated(fragmentData);
            _gameStateService.SetState(GameState.FragmentPlaying);
        }
    }
}