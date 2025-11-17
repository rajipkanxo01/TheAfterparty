using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Presentation.Objects
{
    public class ChestSpriteAnimator : MonoBehaviour
    {
        [Header("Chest Settings")]
        [SerializeField] private string chestId;
        
        [Header("Chest Frames")]
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Sprite openingSprite;
        [SerializeField] private Sprite openSprite;

        [Header("Animation Settings")]
        [SerializeField] private float openingDelay = 0.2f;
        [SerializeField] private float finalOpenDelay = 0.2f;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = closedSprite;
        }

        public void PlayOpenAnimation()
        {
            StartCoroutine(OpenRoutine());
        }

        private IEnumerator OpenRoutine()
        {
            // step 1 — closed → opening
            _renderer.sprite = openingSprite;
            yield return new WaitForSeconds(openingDelay);

            // step 2 — opening → open
            _renderer.sprite = openSprite;
            yield return new WaitForSeconds(finalOpenDelay);

            ChestEvents.RaiseChestOpened(chestId); 
        }
    }
}