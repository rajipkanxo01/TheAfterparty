using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Presentation.Memory.Echoes;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Echo
{
    public class MemoryEchoManager : MonoBehaviour
    {
        private List<MemoryEchoView> _echoes;

        private void Awake()
        {
            _echoes = FindObjectsByType<MemoryEchoView>(FindObjectsSortMode.None)
                .OrderBy(e => e.Order)
                .ToList();

            // Lock all except first
            for (int i = 0; i < _echoes.Count; i++)
            {
                _echoes[i].SetUnlocked(i == 0);
            }

            FragmentEvents.OnFragmentCompleted += HandleFragmentCompleted;
        }

        private void OnDestroy()
        {
            FragmentEvents.OnFragmentCompleted -= HandleFragmentCompleted;
        }

        private void HandleFragmentCompleted(FragmentData completed)
        {
            int index = _echoes.FindIndex(e => e.Order == completed.orderInMemory);
            if (index < 0) return;

            int nextIndex = index + 1;
            if (nextIndex < _echoes.Count)
            {
                Debug.Log($"Unlocking echo #{nextIndex}");
                _echoes[nextIndex].SetUnlocked(true);
            }
        }
    }
}