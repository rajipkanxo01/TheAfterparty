using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory.Echoes
{
    public class MemoryEchoView : MonoBehaviour, IInteractable
    {
        [SerializeField] private MemoryEchoData echoData;
        
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer knotSprite;
        [SerializeField] private SpriteRenderer glowSprite;
        
        [Header("Settings")]
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseIntensity = 0.3f;
        [SerializeField] private float bobSpeed = 1f;
        [SerializeField] private float bobAmount = 0.05f;
        
        [Header("Prompt UI")]
        [SerializeField] private TMPro.TextMeshPro mainText;
        [SerializeField] private TMPro.TextMeshPro glowText;
        [SerializeField] private float fadeSpeed = 4f;
        
        private Color _baseGlowColor;
        private Vector3 _startPos;
        private float _currentAlpha = 0f;
        private bool _playerNear = false;

        private void Awake()
        {
            _startPos = transform.localPosition;
            _baseGlowColor = glowSprite.color;

            if (mainText != null)
            {
                var c = mainText.color;
                c.a = 0f;
                mainText.color = c;
            }

            if (glowText != null)
            {
                var c = glowText.color;
                c.a = 0f;
                glowText.color = c;
            }
        }

        
        private void Update()
        {
            HandlePromptFade();
            HandleGlowPulse();
            HandleBobbing();
        }

        private void HandlePromptFade()
        {
            float target = _playerNear ? 1f : 0f;
            _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);

            if (mainText != null)
            {
                var c = mainText.color;
                c.a = _currentAlpha;
                mainText.color = c;
            }

            if (glowText != null)
            {
                var c = glowText.color;
                c.a = _currentAlpha;
                glowText.color = c;
            }
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
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _playerNear = true;
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _playerNear = false;
            }
        }
        
        public void Interact(GameObject interactor)
        {
            FragmentEvents.RaisePlayFragmentRequested(echoData.fragmentId);
            
        }

        public string GetInteractionPrompt()
        {
            return "Interact (Press E)";
        }
    }
}