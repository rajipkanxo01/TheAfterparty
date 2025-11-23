using System.Collections;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts.Presentation.Memory.Echoes
{
    public class MemoryEchoView : MonoBehaviour, IInteractable
    {
        [SerializeField] private MemoryEchoData echoData;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer knotSprite;
        [SerializeField] private SpriteRenderer glowSprite;
        [SerializeField] private Volume glitchVolume;
        [SerializeField] private ParticleSystem pSystem;

        [Header("Settings")]
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseIntensity = 0.3f;
        [SerializeField] private float bobSpeed = 1f;
        [SerializeField] private float bobAmount = 0.05f;
        [SerializeField] private int order;
        [SerializeField] private bool isUnlocked;

        [Header("Prompt UI")]
        [SerializeField] private TMPro.TextMeshPro mainText;
        [SerializeField] private TMPro.TextMeshPro glowText;
        [SerializeField] private float fadeSpeed = 4f;
        [SerializeField] private float echoFadeDuration = 0.4f;

        private Color _baseGlowColor;
        private Vector3 _startPos;
        private float _currentAlpha = 0f;
        private bool _playerNear = false;

        private GameStateService _gameStateService;
        private Coroutine _fadeRoutine;
        private PlayerProfile _playerProfile;

        public int Order => order;
        public MemoryEchoData EchoData => echoData;
        public bool IsUnlocked => isUnlocked;

        private void Awake()
        {
            _startPos = transform.localPosition;
            _baseGlowColor = glowSprite.color;

            SetPromptAlpha(0f);
        }

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            
            ApplyUnlockVisualState();
        }

        // --------------------------------------------------------------------
        // Unlocking & Visual State
        // --------------------------------------------------------------------

        public void SetUnlocked(bool value)
        {
            isUnlocked = value;
            ApplyUnlockVisualState();
        }

        private void ApplyUnlockVisualState()
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);

            if (!isUnlocked)
            {
                SetEchoAlpha(0f);
                SetPromptAlpha(0f);
                _playerNear = false;
                return;
            }

            if (glitchVolume) glitchVolume.enabled = true;
            if (pSystem) pSystem.Play();
            _fadeRoutine = StartCoroutine(FadeEcho(1f, echoFadeDuration));
        }

        // --------------------------------------------------------------------
        // Fade Animations
        // --------------------------------------------------------------------

        public void HideDuringPlayback()
        {
            /*if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeEcho(0f, echoFadeDuration));
            _playerNear = false;*/
            
            knotSprite.enabled = false;
            glowSprite.enabled = false;
            mainText.enabled = false;
            glowText.enabled = false;
            
            GetComponentInChildren<CircleCollider2D>().enabled = false;
        }

        public void ShowAfterPlayback()
        {
            knotSprite.enabled = true;
            glowSprite.enabled = true;
            mainText.enabled = true;
            glowText.enabled = true;
            
            GetComponentInChildren<CircleCollider2D>().enabled = true;
        }

        private IEnumerator FadeEcho(float targetAlpha, float duration)
        {
            float t = 0f;

            float startKnot = knotSprite.color.a;
            float startGlow = glowSprite.color.a;
            float startPrompt = mainText.color.a;

            while (t < duration)
            {
                float lerp = t / duration;

                float a = Mathf.Lerp(startKnot, targetAlpha, lerp);
                SetEchoAlpha(a);

                float promptA = Mathf.Lerp(startPrompt, targetAlpha, lerp);
                SetPromptAlpha(promptA);

                t += Time.deltaTime;
                yield return null;
            }

            SetEchoAlpha(targetAlpha);
            SetPromptAlpha(targetAlpha);

            _fadeRoutine = null;
        }

        private void SetEchoAlpha(float alpha)
        {
            SetAlpha(knotSprite, alpha);
            SetAlpha(glowSprite, alpha);
        }

        private void SetPromptAlpha(float alpha)
        {
            if (mainText)
            {
                var c = mainText.color;
                c.a = alpha;
                mainText.color = c;
            }

            if (glowText)
            {
                var c = glowText.color;
                c.a = alpha;
                glowText.color = c;
            }

            _currentAlpha = alpha;
        }

        private void SetAlpha(SpriteRenderer sr, float a)
        {
            if (!sr) return;
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }

        // --------------------------------------------------------------------
        // Idle animation
        // --------------------------------------------------------------------

        private void Update()
        {
            if (!isUnlocked) return;

            HandlePromptFade();
            HandleGlowPulse();
            HandleBobbing();
        }

        private void HandlePromptFade()
        {
            float target = _playerNear ? 1f : 0f;
            float newAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);
            SetPromptAlpha(newAlpha);
        }

        private void HandleGlowPulse()
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(_baseGlowColor.a, _baseGlowColor.a + pulseIntensity, t);

            var c = _baseGlowColor;
            c.a = alpha;
            glowSprite.color = c;
        }

        private void HandleBobbing()
        {
            float offset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.localPosition = _startPos + new Vector3(0, offset, 0);
        }

        // --------------------------------------------------------------------
        // Trigger + Interaction
        // --------------------------------------------------------------------

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!isUnlocked) return;
            if (!_gameStateService.IsState(GameState.Normal)) return;

            if (col.CompareTag("Player"))
                _playerNear = true;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
                _playerNear = false;
        }

        public void Interact(GameObject interactor)
        {
            if (!isUnlocked) return;
            if (!_gameStateService.IsState(GameState.Normal)) return;

            FragmentEvents.RaisePlayFragmentRequested(echoData.fragmentId);
            if(glitchVolume) glitchVolume.enabled = false;
            if (pSystem) pSystem.Stop();
        }

        public string GetInteractionPrompt()
        {
            return "Interact (Press E)";
        }
    }
}
