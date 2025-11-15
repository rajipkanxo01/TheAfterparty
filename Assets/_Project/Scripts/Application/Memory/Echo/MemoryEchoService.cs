using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Echo
{
    public class MemoryEchoService
    {
        private readonly FragmentDatabase _fragmentDatabase;
        public MemoryEchoService()
        {
            FragmentEvents.OnPlayFragmentRequested += HandlePlayFragmentRequested;
            
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
        }
    }
}