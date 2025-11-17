using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Presentation.Npc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Dialogue
{
    internal class BubbleInstance
    {
        public GameObject GameObject;
        public Transform Transform;
        public Transform Target;
        public CanvasGroup CanvasGroup;
        public float WobbleX;
        public float WobbleY;
        public Coroutine ScaleCoroutine;
    }

    public class SpeechBubbleView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject bubblePrefab;
        [SerializeField] private Camera mainCamera;

        [Header("Settings")] 
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private float scaleDuration = 0.5f;

        [Header("Wobble Settings")]
        [SerializeField] private float wobbleAmplitudeX = 4f;
        [SerializeField] private float wobbleAmplitudeY = 6f;
        [SerializeField] private float wobbleSpeedX = 2f;
        [SerializeField] private float wobbleSpeedY = 3f;  

        private readonly Dictionary<string, BubbleInstance> _activeBubbles = new Dictionary<string, BubbleInstance>();
        private string _lastSpeakerId;
        private DialogueController _dialogueController;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }

        

        private void Start()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            _activeBubbles.Clear();
        }

        private void OnEnable()
        {
            DialogueEvents.OnDialogueLineStarted += HandleDialogueLineStarted;
            DialogueEvents.OnDialogueEnded += HandleDialogueEnded;
            DialogueEvents.OnDialogueContinued += HandleDialogueContinued;
            DialogueController.OnTrySkipTyping += TrySkipTyping;
        }

        private void OnDisable()
        {
            DialogueEvents.OnDialogueLineStarted -= HandleDialogueLineStarted;
            DialogueEvents.OnDialogueEnded -= HandleDialogueEnded;
            DialogueEvents.OnDialogueContinued -= HandleDialogueContinued;
            DialogueController.OnTrySkipTyping -= TrySkipTyping;
        }

        private void HandleDialogueLineStarted(object sender, DialogueLineEventArgs e)
        {
            bool isSameSpeaker = e.SpeakerId == _lastSpeakerId;

            if (!isSameSpeaker)
            {
                foreach (var kvp in _activeBubbles)
                {
                    if (kvp.Key == e.SpeakerId) continue;

                    if (kvp.Value.ScaleCoroutine != null)
                        StopCoroutine(kvp.Value.ScaleCoroutine);

                    kvp.Value.ScaleCoroutine = StartCoroutine(AnimateScaleOut(kvp.Value));
                }
            }

            // Create new bubble if it doesn’t exist
            if (!_activeBubbles.TryGetValue(e.SpeakerId, out BubbleInstance bubble))
            {
                var npcAnchor = FindNpcAnchor(e.SpeakerId);
                if (npcAnchor == null)
                {
                    Debug.LogWarning($"SpeechBubbleView: NpcAnchor for '{e.SpeakerId}' not found.");
                    return;
                }

                var target = npcAnchor.HeadPoint;
                
                var parent = target.GetComponent<Transform>();

                var go = Instantiate(bubblePrefab, transform);
                
                var rect = go.transform;
                
                rect.localScale = Vector3.zero;
                rect.localPosition = Vector3.zero;
                
                var ui = go.GetComponent<SpeechBubbleUI>();
                ui.Show(e.SpeakerName, e.LineText, e.Portrait);

                var group = ui.CanvasGroup;
                group.alpha = 1;

                bubble = new BubbleInstance
                {
                    GameObject = go,
                    Transform = rect,
                    Target = target,
                    CanvasGroup = group,
                    WobbleX = UnityEngine.Random.Range(0f, Mathf.PI * 2f),
                    WobbleY = UnityEngine.Random.Range(0f, Mathf.PI * 2f)
                };

                _activeBubbles[e.SpeakerId] = bubble;

                bubble.ScaleCoroutine = StartCoroutine(AnimateScaleIn(bubble));
            }
            else
            {
                // If the bubble already exists:
                bubble.GameObject.SetActive(true);
                var bubbleUI = bubble.GameObject.GetComponent<SpeechBubbleUI>();
                bubbleUI.Show(e.SpeakerName, e.LineText, e.Portrait);

                if (!isSameSpeaker)
                {
                    if (bubble.ScaleCoroutine != null)
                        StopCoroutine(bubble.ScaleCoroutine);

                    bubble.ScaleCoroutine = StartCoroutine(AnimateScaleIn(bubble));
                }
            }

            _lastSpeakerId = e.SpeakerId;
        }
        
        
        private void HandleDialogueContinued()
        {
            foreach (var bubble in _activeBubbles.Values)
            {
                bubble.GameObject.SetActive(false);
            }
        }

        private void HandleDialogueEnded()
        {
            foreach (var bubble in _activeBubbles.Values)
            {
                if (bubble.ScaleCoroutine != null)
                {
                    StopCoroutine(bubble.ScaleCoroutine);
                }
            }

            // Then destroy all bubbles
            foreach (var bubble in _activeBubbles.Values)
            {
                if (bubble.GameObject != null)
                {
                    Destroy(bubble.GameObject);
                }
            }

            _activeBubbles.Clear();
        }

        private NpcAnchor FindNpcAnchor(string speakerId)
        {
            var anchors = FindObjectsByType<NpcAnchor>(FindObjectsSortMode.None);
            
            foreach (var a in anchors)
            {
                if (a.NpcId.Trim().Equals(speakerId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return a;
                }
            }
            return null;
        }

        private void LateUpdate()
        {
            foreach (var bubble in _activeBubbles.Values)
            {
                if (bubble.Target == null || bubble.Transform == null || !bubble.GameObject.activeSelf)
                    continue;

                // Animate wobble per bubble
                bubble.WobbleX += Time.deltaTime * wobbleSpeedX;
                bubble.WobbleY += Time.deltaTime * wobbleSpeedY;

                float xOffset = Mathf.Sin(bubble.WobbleX) * wobbleAmplitudeX;
                float yOffset = Mathf.Sin(bubble.WobbleY) * wobbleAmplitudeY;

                // Correctly use the bubble’s own target instead of _currentTarget
                Vector3 worldPos = bubble.Target.position 
                                   + new Vector3(xOffset, yOffset, 0) * 0.01f
                                   + worldOffset;
                bubble.Transform.position = worldPos;

            }
        }
        
        private IEnumerator AnimateScaleIn(BubbleInstance bubble)
        {
            Transform rect = bubble.Transform;
            Vector3 start = Vector3.zero;
            Vector3 end = new Vector3(0.25f, 0.25f, 0.25f);

            float t = 0;
            while (t < scaleDuration)
            {
                t += Time.deltaTime;
                float normalized = t / scaleDuration;
                float ease = 1f - Mathf.Pow(1f - normalized, 3f);
                rect.localScale = Vector3.LerpUnclamped(start, end, ease);
                yield return null;
            }
            
            rect.localScale = end;
        }

        private IEnumerator AnimateScaleOut(BubbleInstance bubble)
        {
            if (bubble == null || bubble.Transform == null)
            {
                yield break;
            }

            Transform rect = bubble.Transform;
            Vector3 start = rect.localScale;
            Vector3 end = Vector3.zero;

            float t = 0;
            while (t < scaleDuration)
            {
                if (rect == null) yield break;
                t += Time.deltaTime;
                float normalized = t / scaleDuration;
                float ease = 1f - Mathf.Pow(1f - normalized, 3f);
                rect.localScale = Vector3.LerpUnclamped(start, end, ease);
                yield return null;
            }

            if (rect != null)
            {
                rect.localScale = end;
            }

            if (bubble.GameObject != null)
            {
                bubble.GameObject.SetActive(false);
            }
        }
        
        public bool TrySkipTyping()
        {
            foreach (var b in _activeBubbles.Values)
            {
                var ui = b.GameObject.GetComponent<SpeechBubbleUI>();
                if (ui != null && ui.IsTyping)
                {
                    // Instantly reveal all text
                    ui.SkipTyping();
                    return true; // we consumed the continue press
                }
            }

            // No bubble needed skipping
            return false;
        }


        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reassign camera whenever a new scene loads
            mainCamera = Camera.main;
        }
    }
}
