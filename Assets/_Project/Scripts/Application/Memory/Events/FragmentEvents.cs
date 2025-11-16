using System;
using _Project.Scripts.Data.Memory.Fragments;

namespace _Project.Scripts.Application.Memory.Events
{
    public static class FragmentEvents
    {
        // Player interacted with a memory echo
        public static event Action<string> OnPlayFragmentRequested;
        public static void RaisePlayFragmentRequested(string fragmentId) 
            => OnPlayFragmentRequested?.Invoke(fragmentId);
        
        // Echo service gets fragment from database
        public static event Action<FragmentData> OnFragmentActivated;
        public static void RaiseFragmentActivated(FragmentData fragment)
            => OnFragmentActivated?.Invoke(fragment);

        // Service says we should start playing it
        public static event Action<FragmentData> OnPlayFragmentStarted;
        public static void RaisePlayFragmentStarted(FragmentData fragment)
            => OnPlayFragmentStarted?.Invoke(fragment);

        // Executor finished playing fragment
        public static event Action<FragmentData> OnFragmentCompleted;
        public static void RaiseFragmentCompleted(FragmentData fragment)
            => OnFragmentCompleted?.Invoke(fragment);
    }
}