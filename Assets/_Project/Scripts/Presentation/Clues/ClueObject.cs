using System.Collections;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Clues;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts.Presentation.Clues
{
    public class ClueObject : MonoBehaviour, IInteractable
    {
        [Header("Clue Info")]
        [SerializeField] private string clueId;

        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float pulseSpeed = 2.5f;
        [SerializeField] private int pulseCount = 2;
        
        [Header("Unlock Condition")]
        [SerializeField] private string requiredMemoryId;
        
        private SpriteRenderer _renderer;
        private Color _originalColor;
        private Coroutine _highlightRoutine;
        private ClueService _clueService;
        private AudioSource _audioSource;
        private bool _isUnlocked;
        private PlayerProfile _playerProfile;

        public string ClueId => clueId;

        [SerializeField] private GameObject puzzleObj;
        private GameStateService _gameStateService;
        [SerializeField] private Volume glitchVolume;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _originalColor = _renderer.color;
            _renderer.enabled = false;

            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        private void Start()
        {
            _clueService = ServiceLocater.GetService<ClueService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            
            if (_clueService is null)
            {
                Debug.LogError("ClueObject: ClueService not found in scene.");
            }
            
            if (_clueService != null && _clueService.IsClueDiscovered(clueId))
            {
                Debug.Log($"[ClueObject] Clue '{clueId}' already discovered — skipping spawn.");
                Destroy(gameObject);
            }
            
            if (string.IsNullOrEmpty(requiredMemoryId) || _playerProfile.HasUnlockedMemory(requiredMemoryId))
            {
                UnlockClue();
            }
        }
        
        private void UnlockClue()
        {
            _isUnlocked = true;
            _renderer.enabled = true;
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
            
            _clueService.DiscoverClue(discoveredClue.clueId);

            
            Destroy(gameObject, 0f);
        }

        private void HandleHintFound(ClueData clueData)
        {
            if (!_isUnlocked || clueData.clueId != clueId) return;
            if (_highlightRoutine != null)
            {
                StopCoroutine(_highlightRoutine);
            };
            
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

            _gameStateService.SetState(GameState.Paused);
            puzzleObj.SetActive(true);
        }

        public void FinishedPuzzle()
        {
            _gameStateService.SetState(GameState.Normal);
            glitchVolume.enabled = false;
            _clueService.Examine(clueId);
        }

        public string GetInteractionPrompt()
        {
            return "Examine Clue";
        }
    }
}
