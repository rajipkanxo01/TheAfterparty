using System;
using System.Collections;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Presentation.Npc;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class SpeechBubbleView : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject bubblePrefab;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private DialogueController dialogueController;

        [Header("Settings")] [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
        [SerializeField] private float fadeDuration = 0.5f;

        private GameObject _currentBubble;
        private RectTransform _bubbleRectTransform;
        private Transform _currentTarget;
        private CanvasGroup _bubbleGroup;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (dialogueController == null)
            {
                Debug.LogError("SpeechBubbleView: DialogueController not assigned.");
                return;
            }

            dialogueController.OnDialogueLineStarted += HandleDialogueLineStarted;
            dialogueController.OnDialogueEnded += HandleDialogueEnded;
            dialogueController.OnDialogueContinued += HandleDialogueContinued;
        }

        private void OnDisable()
        {
            if (dialogueController == null) return;

            dialogueController.OnDialogueLineStarted -= HandleDialogueLineStarted;
            dialogueController.OnDialogueEnded -= HandleDialogueEnded;
            dialogueController.OnDialogueContinued -= HandleDialogueContinued;
        }

        private void HandleDialogueLineStarted(object sender, DialogueLineEventArgs e)
        {
            var npcAnchor = FindNpcAnchor(e.SpeakerId);
            if (npcAnchor == null)
            {
                Debug.LogWarning($"SpeechBubbleView: NpcAnchor for '{e.SpeakerId}' not found.");
                return;
            }

            _currentTarget = npcAnchor.HeadPoint != null ? npcAnchor.HeadPoint : npcAnchor.transform;
            
            _currentBubble = Instantiate(bubblePrefab, transform);
            _bubbleRectTransform = _currentBubble.GetComponent<RectTransform>();
            
            
            var bubbleUI = _currentBubble.GetComponent<SpeechBubbleUI>();
            Vector3 screenPos = mainCamera.WorldToScreenPoint(_currentTarget.position + offset);
            _bubbleRectTransform.position = screenPos;
            
            bubbleUI.DialogueText.text = e.LineText;
            bubbleUI.PortraitImage.sprite = e.Portrait;
            
            _bubbleGroup = bubbleUI.CanvasGroup;
            bubbleUI.CanvasGroup.alpha = 0;
            
            _currentBubble.SetActive(true);

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeInBubble());
        }

        private void HandleDialogueContinued()
        {
            if (_currentBubble != null)
            {
                if (_fadeCoroutine != null)
                    StopCoroutine(_fadeCoroutine);

                _fadeCoroutine = StartCoroutine(FadeOutAndDestroy());
            }
        }

        private void HandleDialogueEnded()
        {
            if (_currentBubble != null)
            {
                Destroy(_currentBubble);
            }
        }

        private NpcAnchor FindNpcAnchor(string speakerId)
        {
            NpcAnchor[] anchors = { FindAnyObjectByType<NpcAnchor>() };
            foreach (var npcAnchor in anchors)
            {
                if (npcAnchor.NpcId.Equals(speakerId, StringComparison.OrdinalIgnoreCase))
                    return npcAnchor;
            }

            return null;
        }

        private IEnumerator FadeInBubble()
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                if (_bubbleGroup == null) yield break;
                t += Time.deltaTime;
                _bubbleGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
        }

        private IEnumerator FadeOutAndDestroy()
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                if (_bubbleGroup == null) yield break;
                t += Time.deltaTime;
                _bubbleGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }

            if (_currentBubble != null)
                Destroy(_currentBubble);
        }

        private void LateUpdate()
        {
            if (_currentBubble != null && _currentTarget != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(_currentTarget.position);
                _bubbleRectTransform.position = screenPos;
            }
        }
    }
}