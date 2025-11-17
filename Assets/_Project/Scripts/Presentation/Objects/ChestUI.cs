using System;
using System.Collections;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Presentation.Objects
{
    public static class ChestEvents
    {
        public static event Action<string> OnChestOpened;

        public static void RaiseChestOpened(string chestId)
        {
            OnChestOpened?.Invoke(chestId);
        }
    }

    public class ChestUI : MonoBehaviour
    {
        private static ChestUI _instance;

        [SerializeField] private GameObject root;
        [SerializeField] private TMPro.TextMeshProUGUI display;

        [Header("FX Settings")]
        [SerializeField] private float flashDuration = 0.25f;
        [SerializeField] private float shakeAmount = 10f;
        [SerializeField] private float shakeDuration = 0.25f;

        private ChestData _currentChest;
        private string _input = "";
        private Color _defaultColor;
        private GameStateService _gameStateService;

        private void Awake()
        {
            _instance = this;
            root.SetActive(false);

            if (display != null)
            {
                _defaultColor = display.color;
            }
        }

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        public static void Open(ChestData chest)
        {
            _instance._currentChest = chest;
            _instance._input = "";
            _instance.display.text = "";
            _instance.display.color = _instance._defaultColor;

            _instance.root.SetActive(true);
        }

        public void PressDigit(string digit)
        {
            _input += digit;
            display.text = _input;

            if (_input.Length == _currentChest.correctCode.Length)
            {
                _instance.StartCoroutine(_instance.ValidateRoutine());
            }
        }

        private IEnumerator ValidateRoutine()
        {
            // Small pause before checking
            yield return new WaitForSeconds(0.2f);

            if (_input == _currentChest.correctCode)
            {
                // GREEN FLASH
                yield return StartCoroutine(GreenFlash());

                // Small delay before closing
                yield return new WaitForSeconds(0.2f);

                
                Close();
                
                ChestSpriteAnimator animator = FindAnyObjectByType<ChestSpriteAnimator>();
                if (animator != null)
                {
                    animator.PlayOpenAnimation();
                }
                else
                {
                    Debug.LogWarning("ChestSpriteAnimator not found in scene!");
                }
                
            }
            else
            {
                // SHAKE + RED FLASH
                yield return StartCoroutine(Shake());
                yield return StartCoroutine(RedFlash());

                // Reset wrong input
                _input = "";
                display.text = "";
                display.color = _defaultColor;
            }
        }

        private IEnumerator RedFlash()
        {
            display.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            display.color = _defaultColor;
        }

        private IEnumerator GreenFlash()
        {
            display.color = Color.green;
            yield return new WaitForSeconds(flashDuration);
            display.color = _defaultColor;
        }

        private IEnumerator Shake()
        {
            RectTransform rect = display.rectTransform;
            Vector3 originalPos = rect.anchoredPosition;

            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-shakeAmount, shakeAmount);
                float y = Random.Range(-shakeAmount, shakeAmount);

                rect.anchoredPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            rect.anchoredPosition = originalPos;
        }

        public void Close()
        {
            _gameStateService.SetState(GameState.Normal);
            root.SetActive(false);
        }
    }
}
