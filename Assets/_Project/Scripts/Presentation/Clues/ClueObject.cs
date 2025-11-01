using System.Collections;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;
using UnityEngine;

namespace _Project.Scripts.Presentation.Clues
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ClueObject : MonoBehaviour, IInteractable
    {
        [Header("Clue Info")]
        [SerializeField] private string clueId;

        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float pulseSpeed = 2.5f;
        [SerializeField] private int pulseCount = 2;

        [Header("Effects")]
        // [SerializeField] private ParticleSystem discoverEffect;
        
        private SpriteRenderer _renderer;
        private Color _originalColor;
        private Coroutine _highlightRoutine;
        private ClueService _clueService;
        private AudioSource _audioSource;

        public string ClueId => clueId;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _originalColor = _renderer.color;

        }

        private void Start()
        {
            _clueService = ServiceLocater.GetService<ClueService>();
            if (_clueService == null)
            {
                Debug.LogError("ClueObject: ClueService not found in scene.");
            }
        }

        private void OnEnable()
        {
            ClueEvents.OnClueDiscovered += HandleClueDiscovered;
            ClueEvents.OnHintFound += HandleHintFound;
        }

        private void OnDisable()
        {
            ClueEvents.OnClueDiscovered -= HandleClueDiscovered;
            ClueEvents.OnHintFound -= HandleHintFound;
        }

        private void HandleClueDiscovered(ClueData discoveredClue)
        {
            if (discoveredClue.clueId != clueId) return;

            /*if (discoverEffect != null)
            {
                Instantiate(discoverEffect, transform.position, Quaternion.identity);
            }*/
            
            ToastNotification.Show($"Clue '{discoveredClue.clueName}' added to journal.", "success");

            // Delay destroy slightly if there's an effect
            Destroy(gameObject, 0f);
            // Destroy(gameObject, discoverEffect != null ? 0.5f : 0f);
        }

        private void HandleHintFound(ClueData clueData)
        {
            if (clueData.clueId != clueId) return;
            if (_highlightRoutine != null) StopCoroutine(_highlightRoutine);
            _highlightRoutine = StartCoroutine(PulseHighlight());
        }

        private IEnumerator PulseHighlight()
        {
            float elapsed = 0f;
            for (int i = 0; i < pulseCount; i++)
            {
                // Lerp color in and out smoothly
                while (elapsed < Mathf.PI)
                {
                    float t = (Mathf.Sin(elapsed) + 1f) / 2f; // 0 → 1 → 0 pulse
                    _renderer.color = Color.Lerp(_originalColor, highlightColor, t);
                    elapsed += Time.deltaTime * pulseSpeed;
                    yield return null;
                }
                elapsed = 0f;
            }

            _renderer.color = _originalColor;
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.CompareTag("Player")) return;
            _clueService.Examine(clueId);
        }

        public string GetInteractionPrompt()
        {
            return "Examine Clue";
        }
    }
}
