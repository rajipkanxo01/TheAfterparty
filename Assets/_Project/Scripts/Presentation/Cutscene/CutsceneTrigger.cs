using _Project.Scripts.Application.Cutscene.Events;
using _Project.Scripts.Data.Cutscene;
using UnityEngine;

namespace _Project.Scripts.Presentation.Cutscene
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [Header("Cutscene Settings")]
        [SerializeField] private CutsceneData cutscene;
        
        [Header("Trigger Settings")]
        [SerializeField] private TriggerType triggerType = TriggerType.OnTriggerEnter;
        
        [Tooltip("Tag that must match for trigger to activate (leave empty for any)")]
        [SerializeField] private string requiredTag = "Player";
        
        [Tooltip("If true, this trigger will only fire once and then disable itself")]
        [SerializeField] private bool triggerOnce = true;
        
        [Tooltip("Delay in seconds before playing the cutscene")]
        [SerializeField] private float playDelay = 0f;

        private bool _hasTriggered = false;

        public enum TriggerType
        {
            OnTriggerEnter,
            OnCollisionEnter,
            OnStart,
            Manual
        }

        private void Start()
        {
            if (cutscene == null)
            {
                Debug.LogWarning($"CutsceneTrigger on {gameObject.name}: No cutscene assigned");
                return;
            }

            if (triggerType == TriggerType.OnStart)
            {
                // Delay slightly to ensure CutsceneManager has initialized
                StartCoroutine(TriggerOnStartDelayed());
            }
        }

        private System.Collections.IEnumerator TriggerOnStartDelayed()
        {
            // Wait for end of frame to ensure all Start() methods have been called
            yield return new WaitForEndOfFrame();
            PlayCutscene();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerType != TriggerType.OnTriggerEnter) return;
            
            if (ShouldTrigger(other.tag))
            {
                PlayCutscene();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (triggerType != TriggerType.OnCollisionEnter) return;
            
            if (ShouldTrigger(collision.gameObject.tag))
            {
                PlayCutscene();
            }
        }

        private bool ShouldTrigger(string objectTag)
        {
            if (_hasTriggered && triggerOnce)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(requiredTag) && objectTag != requiredTag)
            {
                return false;
            }

            return true;
        }
        
        public void TriggerCutscene()
        {
            if (_hasTriggered && triggerOnce)
            {
                Debug.Log($"CutsceneTrigger on {gameObject.name}: Already triggered");
                return;
            }

            PlayCutscene();
        }

        private void PlayCutscene()
        {
            if (cutscene == null)
            {
                Debug.LogError($"CutsceneTrigger on {gameObject.name}: Cannot play null cutscene");
                return;
            }

            _hasTriggered = true;

            if (playDelay > 0)
            {
                Invoke(nameof(RequestCutscenePlay), playDelay);
            }
            else
            {
                RequestCutscenePlay();
            }

            if (triggerOnce)
            {
                // Disable the trigger component
                enabled = false;
            }
        }

        private void RequestCutscenePlay()
        {
            Debug.Log($"CutsceneTrigger on {gameObject.name}: Requesting cutscene {cutscene.cutsceneId}");
            CutsceneEvents.RaisePlayCutsceneRequested(cutscene.cutsceneId);
        }

        private void OnDrawGizmos()
        {
            // Draw a visual indicator for the trigger
            if (triggerType == TriggerType.OnTriggerEnter || triggerType == TriggerType.OnCollisionEnter)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                Gizmos.DrawWireCube(transform.position, Vector3.one);
            }
        }
    }
}

