using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class SpeechBubbleView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text continueHint;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform target;

        [Header("Settings")] [SerializeField] private Vector3 offset = new(0, 2f, 0);
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float followSpeed = 5f;

        private Camera _camera;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _camera = Camera.main;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }

            if (continueHint != null)
            {
                continueHint.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // Smoothly follow the target position with offset
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

            // Face the camera
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        }

        public void AttachTo(Transform npcHead)
        {
            target = npcHead;
        }

        public void Show(string speakerName, string text)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            speakerNameText.text = speakerName;
            dialogueText.text = text;
            _fadeCoroutine = StartCoroutine(FadeCanvas(1f));
        }

        public void Hide()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (continueHint != null)
            {
                continueHint.gameObject.SetActive(false);
            }

            _fadeCoroutine = StartCoroutine(FadeCanvas(0f));
        }

        public void ShowContinueHint()
        {
            if (continueHint != null)
            {
                continueHint.gameObject.SetActive(true);
            }
        }

        public void HideContinueHint()
        {
            if (continueHint != null)
            {
                continueHint.gameObject.SetActive(false);
            }
        }

        private IEnumerator FadeCanvas(float duration)
        {
            float start = canvasGroup.alpha;
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, duration, t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = duration;
        }
    }
}