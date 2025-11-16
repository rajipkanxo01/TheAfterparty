using System.Collections;
using UnityEngine;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Utilities;
using _Project.Scripts.Data.Memory.Fragments;

namespace _Project.Scripts.Presentation.Npc
{
    public class MemoryNpcController : MonoBehaviour
    {
        [Header("NPC Identity")]
        [SerializeField] private string npcId;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.8f;
        [SerializeField] private bool freezeBeforeFadeOut = false;

        [Tooltip("Optional: particle fx played when fading out (memory dissolve).")]
        [SerializeField] private ParticleSystem fadeOutParticles;

        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        private Coroutine _fadeRoutine;
        private bool _isVisible = false;

        private string LogPrefix => $"[MemoryNpcController:{npcId}]";


        // ---------------- LIFECYCLE ----------------

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                Debug.Log($"{LogPrefix} SpriteRenderer assigned from children: {(spriteRenderer != null)}");
            }

            SetVisibleInstant(false);
        }

        private void OnEnable()
        {
            FragmentEvents.OnPlayFragmentStarted += HandleFragmentStart;
            FragmentEvents.OnFragmentCompleted += HandleFragmentEnd;
        }

        private void OnDisable()
        {
            FragmentEvents.OnPlayFragmentStarted -= HandleFragmentStart;
            FragmentEvents.OnFragmentCompleted -= HandleFragmentEnd;
        }


        // ---------------- EVENT REACTIONS ----------------

        private void HandleFragmentStart(FragmentData fragment)
        {
            if (!FragmentNpcUtility.ContainsNpc(fragment, npcId))
            {
                Debug.Log($"{LogPrefix} HandleFragmentStart - fragment does not contain npc '{npcId}', ignoring");
                return;
            }

            Vector3 startPos = FragmentNpcUtility.GetStartingPosition(fragment, npcId);
            transform.position = startPos;

            FadeIn();
        }

        private void HandleFragmentEnd(FragmentData fragment)
        {
            if (!FragmentNpcUtility.ContainsNpc(fragment, npcId))
            {
                Debug.Log($"{LogPrefix} HandleFragmentEnd - fragment does not contain npc '{npcId}', ignoring");
                return;
            }

            FadeOut();
        }


        // ---------------- VISIBILITY METHODS ----------------

        private void FadeIn()
        {
            if (_fadeRoutine != null)
            {
                Debug.Log($"{LogPrefix} FadeIn - stopping existing fade routine");
                StopCoroutine(_fadeRoutine);
            }

            _fadeRoutine = StartCoroutine(FadeRoutine(0f, 1f));
            _isVisible = true;
        }

        private void FadeOut()
        {
            if (_fadeRoutine != null)
            {
                Debug.Log($"{LogPrefix} FadeOut - stopping existing fade routine");
                StopCoroutine(_fadeRoutine);
            }

            if (freezeBeforeFadeOut && animator != null)
            {
                animator.enabled = false;
                Debug.Log($"{LogPrefix} FadeOut - animator frozen");
            }

            if (fadeOutParticles != null)
            {
                fadeOutParticles.transform.position = transform.position;
                fadeOutParticles.Play();
                Debug.Log($"{LogPrefix} FadeOut - played fadeOutParticles at {transform.position}");
            }

            _fadeRoutine = StartCoroutine(FadeRoutine(1f, 0f, () => SetVisibleInstant(false)));
            _isVisible = false;
        }

        private IEnumerator FadeRoutine(float from, float to, System.Action onFinish = null)
        {
            SetVisibleInstant(true);

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(from, to, t / fadeDuration);

                if (spriteRenderer != null)
                {
                    var c = spriteRenderer.color;
                    c.a = alpha;
                    spriteRenderer.color = c;
                }

                yield return null;
            }

            onFinish?.Invoke();
        }

        private void SetVisibleInstant(bool visible)
        {
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"{LogPrefix} SetVisibleInstant called but spriteRenderer is null");
                return;
            }

            var c = spriteRenderer.color;
            c.a = visible ? 1f : 0f;
            spriteRenderer.color = c;

            gameObject.SetActive(true);
        }
    }
}