using _Project.Scripts.Application.Cutscene.Events;
using UnityEngine;

namespace _Project.Scripts.Presentation.Cutscene
{
    public class CutsceneDebugHelper : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugKeys = true;
        
        [Header("Cutscenes")]
        [SerializeField] private string cutsceneId1 = "";
        [SerializeField] private KeyCode playCutscene1Key = KeyCode.F3;
        
        [SerializeField] private string cutsceneId2 = "";
        [SerializeField] private KeyCode playCutscene2Key = KeyCode.Alpha2;
        
        [SerializeField] private string cutsceneId3 = "";
        [SerializeField] private KeyCode playCutscene3Key = KeyCode.Alpha3;

        private void Update()
        {
            if (!enableDebugKeys) return;

            if (!string.IsNullOrEmpty(cutsceneId1) && Input.GetKeyDown(playCutscene1Key))
            {
                Debug.Log($"CutsceneDebugHelper: Playing cutscene {cutsceneId1}");
                CutsceneEvents.RaisePlayCutsceneRequested(cutsceneId1);
            }

            if (!string.IsNullOrEmpty(cutsceneId2) && Input.GetKeyDown(playCutscene2Key))
            {
                Debug.Log($"CutsceneDebugHelper: Playing cutscene {cutsceneId2}");
                CutsceneEvents.RaisePlayCutsceneRequested(cutsceneId2);
            }

            if (!string.IsNullOrEmpty(cutsceneId3) && Input.GetKeyDown(playCutscene3Key))
            {
                Debug.Log($"CutsceneDebugHelper: Playing cutscene {cutsceneId3}");
                CutsceneEvents.RaisePlayCutsceneRequested(cutsceneId3);
            }
        }

        public void PlayCutsceneById(string cutsceneId)
        {
            if (string.IsNullOrEmpty(cutsceneId))
            {
                Debug.LogWarning("CutsceneDebugHelper: Cannot play cutscene with empty ID");
                return;
            }

            Debug.Log($"CutsceneDebugHelper: Playing cutscene {cutsceneId}");
            CutsceneEvents.RaisePlayCutsceneRequested(cutsceneId);
        }
    }
}

