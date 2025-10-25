using System;
using System.Collections;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Data.Npc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private GameObject speechBubblePrefab;
        [SerializeField] private Key key = Key.E;

        private DialogueManager _dialogueManager;
        private SpeechBubbleView _speechBubbleView;
        private Transform _npcTarget;
        private bool _waitingForInput;

        private InputAction _advanceAction;

        public void Initialize(DialogueService dialogueService)
        {
            _dialogueManager = dialogueService.DialogueManager;
            _dialogueManager.OnDialogueLineStarted += HandleDialogueLineStarted;
            _dialogueManager.OnDialogueEnded += HandleDialogueEnded;

            // todo: remove this temp solution and use proper input system setup
            _advanceAction = new InputAction(
                type: InputActionType.Button,
                binding: $"<Keyboard>/{key.ToString().ToLower()}");

            _advanceAction.performed += OnAdvancePerformed;
            _advanceAction.Enable();
        }

        private void OnDestroy()
        {
            if (_advanceAction != null)
            {
                _advanceAction.performed -= OnAdvancePerformed;
                _advanceAction.Disable();
                _advanceAction.Dispose();
            }
        }

        private void OnAdvancePerformed(InputAction.CallbackContext context)
        {
            if (!_waitingForInput) return;

            _waitingForInput = false;
            _speechBubbleView.HideContinueHint();
            _dialogueManager.ShowNextDialogueLine();
        }

        private void HandleDialogueLineStarted(DialogueLine dialogueLine)
        {
            if (_speechBubbleView == null)
            {
                var instance = Instantiate(speechBubblePrefab, Vector3.zero, Quaternion.identity);
                _speechBubbleView = instance.GetComponent<SpeechBubbleView>();
                _speechBubbleView.AttachTo(_npcTarget);
            }

            _speechBubbleView.Show(dialogueLine.speakerName, dialogueLine.text);
            StartCoroutine(WaitThenEnableHint());
        }

        private void HandleDialogueEnded()
        {
            _waitingForInput = false;
            if (_speechBubbleView != null)
                _speechBubbleView.Hide();
        }

        private IEnumerator WaitThenEnableHint()
        {
            yield return new WaitForSeconds(0.5f);
            _speechBubbleView.ShowContinueHint();
            _waitingForInput = true;
        }

        public void SetTarget(Transform target)
        {
            _npcTarget = target;
        }
    }
}
