using System;
using System.Collections;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Memory.Events;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Memory
{
    public class MemoryTransitionView : MonoBehaviour
    {
        [SerializeField] private AudioClip transitionSfx;
        
        [Header("Fade Settings")]
        [SerializeField] private Image fadeOverlay;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float blackHoldDuration = 0.5f;
        [SerializeField] private float whiteFlashDuration = 0.8f;
        [SerializeField] private bool useScaleEffect = true;
        
        private RectTransform _overlayRect;

        private void Awake()
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.color = new Color(0, 0, 0, 0);
                _overlayRect = fadeOverlay.GetComponent<RectTransform>();
            }
            
            ServiceLocater.RegisterService(this);
        }

        public void Play()
        {
            Debug.Log($"TransitionCanvas activeSelf={gameObject.activeSelf}, inHierarchy={gameObject.activeInHierarchy}");
            StartCoroutine(PlayTransition());
        }

        private IEnumerator PlayTransition()
        {
            if (fadeOverlay == null)
            {
                Debug.LogWarning("[MemoryTransitionView] Fade overlay not assigned!");
                yield break;
            }

            // Reset transform
            if (useScaleEffect && _overlayRect != null)
            {
                _overlayRect.localScale = Vector3.one * 3f;
            }
            
            // Play transition sound
            AudioManager.Instance.PlaySfx(transitionSfx);

            // 1. Fade to black + optional shrink to center
            yield return StartCoroutine(FadeToColor(Color.black, fadeDuration, useScaleEffect));

            // 2. Hold full black
            yield return new WaitForSeconds(blackHoldDuration);
            
            // 4. Transition complete (scene load trigger)
            MemoryEvents.RaiseMemoryTransitionEnd();

            // 3. Bright flash
            yield return StartCoroutine(FadeToColor(Color.white, whiteFlashDuration, false));

          

            // 5. Fade back to transparent and restore size
            yield return StartCoroutine(FadeToColor(new Color(0, 0, 0, 0), fadeDuration, useScaleEffect));

        }

        private IEnumerator FadeToColor(Color targetColor, float duration, bool scaleEffect)
        {
            float time = 0;
            Color startColor = fadeOverlay.color;
            Vector3 startScale = _overlayRect != null ? _overlayRect.localScale : Vector3.one;
            Vector3 targetScale = Vector3.one;

            if (scaleEffect)
            {
                // Shrink inward (corners to center) when fading to black
                targetScale = targetColor.a > startColor.a ? Vector3.one : Vector3.one * 3f;
            }

            while (time < duration)
            {
                float t = time / duration;

                fadeOverlay.color = Color.Lerp(startColor, targetColor, t);

                if (scaleEffect && _overlayRect != null)
                {
                    _overlayRect.localScale = Vector3.Lerp(startScale, targetScale, t);
                }

                time += Time.deltaTime;
                yield return null;
            }

            fadeOverlay.color = targetColor;

            if (scaleEffect && _overlayRect != null)
            {
                _overlayRect.localScale = targetScale;
            }
        }
        
        private void OnDestroy()
        {
            Debug.LogWarning("[MemoryTransitionView] Destroyed during runtime!", this);
        }

    }
}
