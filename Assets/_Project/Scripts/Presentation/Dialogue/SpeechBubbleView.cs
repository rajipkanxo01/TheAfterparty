using System;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Presentation.Npc;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class SpeechBubbleView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject bubblePrefab;

        [SerializeField] private Camera mainCamera;

        [Header("Settings")] 
        [SerializeField] private Vector3 screenOffset = new Vector3(0, 10f, 0);

        private GameObject _currentBubble;
        private RectTransform _bubbleRectTransform;
        private Transform _currentTarget;
        private CanvasGroup _bubbleGroup;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            DialogueEvents.OnDialogueLineStarted += HandleDialogueLineStarted;
            DialogueEvents.OnDialogueEnded += HandleDialogueEnded;
            DialogueEvents.OnDialogueContinued += HandleDialogueContinued;
        }

        private void OnDisable()
        {
            DialogueEvents.OnDialogueLineStarted -= HandleDialogueLineStarted;
            DialogueEvents.OnDialogueEnded -= HandleDialogueEnded;
            DialogueEvents.OnDialogueContinued -= HandleDialogueContinued;
        }

        private void HandleDialogueLineStarted(object sender, DialogueLineEventArgs e)
        {
            var npcAnchor = FindNpcAnchor(e.SpeakerId);
            if (npcAnchor == null)
            {
                Debug.LogWarning($"SpeechBubbleView: NpcAnchor for '{e.SpeakerId}' not found.");
                return;
            }

            _currentTarget = npcAnchor.HeadPoint;
            
            _currentBubble = Instantiate(bubblePrefab, transform);
            _bubbleRectTransform = _currentBubble.GetComponent<RectTransform>();
            Vector3 screenPos = mainCamera.WorldToScreenPoint(_currentTarget.position);
            _bubbleRectTransform.position = screenPos + screenOffset;
            
            var bubbleUI = _currentBubble.GetComponent<SpeechBubbleUI>();
            bubbleUI.DialogueText.text = e.LineText;
            bubbleUI.PortraitImage.sprite = e.Portrait;
            bubbleUI.SpeakerText.text = e.SpeakerName;
            
            _bubbleGroup = bubbleUI.CanvasGroup;
            bubbleUI.CanvasGroup.alpha = 0;
            
            _currentBubble.SetActive(true);
            
            _bubbleGroup.alpha = 1;
        }

        private void HandleDialogueContinued()
        {
            if (_currentBubble != null)
            {
                Destroy(_currentBubble);
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
            NpcAnchor[] anchors = FindObjectsByType<NpcAnchor>(FindObjectsSortMode.None);
            foreach (var npcAnchor in anchors)
            {
                if (npcAnchor.NpcId.Equals(speakerId, StringComparison.OrdinalIgnoreCase))
                    return npcAnchor;
            }

            return null;
        }

        private void LateUpdate()
        {
            if (_currentBubble != null && _currentTarget != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(_currentTarget.position);
                _bubbleRectTransform.position = screenPos + screenOffset;
            }
        }
    }
}