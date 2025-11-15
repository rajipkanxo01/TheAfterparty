using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryFragmentService
    {
        private readonly List<FragmentData> _fragments;
        private int _currentIndex = 0;
        private bool _isReplay;

        public MemoryFragmentService(string memoryId)
        {
            var memoryDb = ServiceLocater.GetService<MemoryDatabase>();
            var memory = memoryDb.GetById(memoryId);

            _fragments = memory.fragments;

            FragmentEvents.OnFragmentActivated += HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted += HandleFragmentCompleted;

            Debug.Log($"MemoryFragmentService: Initialized for memory '{memoryId}'. Total fragments: {_fragments.Count}");
        }

        private void HandleFragmentActivated(FragmentData fragment)
        {
            var expected = _fragments[_currentIndex];

            // when replaying earlier fragment
            if (fragment.orderInMemory < expected.orderInMemory)
            {
                Debug.Log($"Replay mode: Replaying '{fragment.fragmentId}'");
                _isReplay = true;
                FragmentEvents.RaisePlayFragmentStarted(fragment);
                return;
            }

            if (fragment.orderInMemory > expected.orderInMemory)
            {
                Debug.Log($"MemoryFragmentService: Wrong fragment activated. Expected '{expected.fragmentId}'");
                return;
            }

            // playing fragment for first time
            Debug.Log($"MemoryFragmentService: Fragment '{fragment.fragmentId}' is correct. Starting...");
            _isReplay = false;
            FragmentEvents.RaisePlayFragmentStarted(fragment);
        }


        private void HandleFragmentCompleted(FragmentData fragment)
        {
            // when replaying, do not advance index
            if (_isReplay)
            {
                Debug.Log($"Replay completed for '{fragment.fragmentId}' — NOT advancing index.");
                _isReplay = false;
                return;
            }

            _currentIndex++;

            if (_currentIndex >= _fragments.Count)
            {
                Debug.Log("MemoryFragmentService: All fragments completed!");
                MemoryEvents.RaiseAllFragmentsCompleted();
                return;
            }

            Debug.Log($"MemoryFragmentService: Next fragment is '{_fragments[_currentIndex].fragmentId}'");
        }

        
        public void Dispose()
        {
            FragmentEvents.OnFragmentActivated -= HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted -= HandleFragmentCompleted;
        }

    }
}