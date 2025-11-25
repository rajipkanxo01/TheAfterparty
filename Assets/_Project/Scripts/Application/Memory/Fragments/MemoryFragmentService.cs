using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Fragments
{
    public class MemoryFragmentService
    {
        private readonly List<FragmentData> _fragments;

        private int _currentIndex = 0;
        private bool _isReplay = false;
        private bool _allFragmentsCompleted = false;

        public MemoryFragmentService(string memoryId)
        {
            var db = ServiceLocater.GetService<MemoryDatabase>();
            var memory = db.GetById(memoryId);

            if (memory == null)
            {
                Debug.LogError($"MemoryFragmentService: Memory '{memoryId}' not found.");
                return;
            }

            _fragments = memory.fragments;

            FragmentEvents.OnFragmentActivated += HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted += HandleFragmentCompleted;

        }

        private void HandleFragmentActivated(FragmentData fragment)
        {
            if (_allFragmentsCompleted)
            {
                FragmentEvents.RaisePlayFragmentStarted(fragment);
                return;
            }
            
            if (_currentIndex < 0 || _currentIndex >= _fragments.Count)
            {
                _currentIndex = 0;
            }

            var expected = _fragments[_currentIndex];

            // 1. Replay of earlier fragment
            if (fragment.orderInMemory < expected.orderInMemory)
            {
                Debug.Log($"Replay earlier fragment: '{fragment.fragmentId}'");
                _isReplay = true;
                FragmentEvents.RaisePlayFragmentStarted(fragment);
                return;
            }

            // 2. Player clicked a future fragment too early
            if (fragment.orderInMemory > expected.orderInMemory)
            {
                Debug.Log($"MemoryFragmentService: Wrong fragment. Expected '{expected.fragmentId}'.");
                return;
            }

            // 3. Correct fragment
            _isReplay = false;
            FragmentEvents.RaisePlayFragmentStarted(fragment);
        }


        private void HandleFragmentCompleted(FragmentData fragment)
        {
            if (_currentIndex < 0 || _currentIndex >= _fragments.Count)
            {
                _currentIndex = Mathf.Clamp(_currentIndex, 0, _fragments.Count - 1);
            }

            if (_isReplay)
            {
                Debug.Log($"Replay completed for '{fragment.fragmentId}'. No index advance.");
                _isReplay = false;
                return;
            }

            _currentIndex++;

            if (_currentIndex >= _fragments.Count)
            {
                _allFragmentsCompleted = true;

                Debug.Log("MemoryFragmentService: All fragments completed! Entering free replay mode.");
                MemoryEvents.RaiseAllFragmentsCompleted();
                return;
            }

            ToastNotification.Show("New memory fragment found! Look around.", 4f);
        }


        public void Dispose()
        {
            FragmentEvents.OnFragmentActivated -= HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted -= HandleFragmentCompleted;
        }
    }
}
