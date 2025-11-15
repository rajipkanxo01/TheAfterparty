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
            if (_fragments[_currentIndex] != fragment)
            {
                Debug.Log($"MemoryFragmentService: Wrong fragment activated. Expected {_fragments[_currentIndex].fragmentId}");
                return;
            }

            Debug.Log($"MemoryFragmentService: Fragment '{fragment.fragmentId}' is correct. Starting...");
            FragmentEvents.RaisePlayFragmentStarted(fragment);
        }

        private void HandleFragmentCompleted(FragmentData fragment)
        {
            _currentIndex++;

            if (_currentIndex >= _fragments.Count)
            {
                Debug.Log("MemoryFragmentService: All fragments completed!");
                MemoryEvents.RaiseAllFragmentsCompleted();
                return;
            }

            Debug.Log($"MemoryFragmentService: Next fragment: {_fragments[_currentIndex].fragmentId}");
        }
        
        public void Dispose()
        {
            FragmentEvents.OnFragmentActivated -= HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted -= HandleFragmentCompleted;
        }

    }
}