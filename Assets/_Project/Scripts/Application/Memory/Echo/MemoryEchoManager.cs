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
        private MemoryEchoView _lastUsedEcho;

        private void Awake()
        {
            _echoes = FindObjectsByType<MemoryEchoView>(FindObjectsSortMode.None)
                        .OrderBy(e => e.Order)
                        .ToList();

            // Unlock only first echo
            for (int i = 0; i < _echoes.Count; i++)
            {
                _echoes[i].SetUnlocked(i == 0);
            }

            FragmentEvents.OnFragmentActivated += HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted += HandleFragmentCompleted;
        }

        private void OnDestroy()
        {
            FragmentEvents.OnFragmentActivated -= HandleFragmentActivated;
            FragmentEvents.OnFragmentCompleted -= HandleFragmentCompleted;
        }


        private void HandleFragmentActivated(FragmentData fragment)
        {
            _lastUsedEcho = _echoes.FirstOrDefault(e => e.EchoData.fragmentId == fragment.fragmentId);

            if (_lastUsedEcho != null)
            {
                _lastUsedEcho.HideDuringPlayback();
            }
        }

        private void HandleFragmentCompleted(FragmentData fragment)
        {
            // show the used echo again (allow replay)
            if (_lastUsedEcho != null)
                _lastUsedEcho.ShowAfterPlayback();

            // unlock next echo
            int index = _echoes.FindIndex(e => e.Order == fragment.orderInMemory);
            if (index < 0) return;

            int nextIndex = index + 1;
            if (nextIndex < _echoes.Count)
            {
                _echoes[nextIndex].SetUnlocked(true);
            }
        }
    }
}
