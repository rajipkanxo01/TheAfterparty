using System;
using _Project.Scripts.Data.Cutscene;

namespace _Project.Scripts.Application.Cutscene.Events
{
    public static class CutsceneEvents
    {
        public static event Action<CutsceneData> OnCutsceneStarted;
        public static event Action<CutsceneData> OnCutsceneCompleted;
        
        public static event Action<string> OnPlayCutsceneRequested;
        
        public static void RaiseCutsceneStarted(CutsceneData cutscene)
        {
            OnCutsceneStarted?.Invoke(cutscene);
        }
        
        public static void RaiseCutsceneCompleted(CutsceneData cutscene)
        {
            OnCutsceneCompleted?.Invoke(cutscene);
        }
        
        public static void RaisePlayCutsceneRequested(string cutsceneId)
        {
            OnPlayCutsceneRequested?.Invoke(cutsceneId);
        }
    }
}