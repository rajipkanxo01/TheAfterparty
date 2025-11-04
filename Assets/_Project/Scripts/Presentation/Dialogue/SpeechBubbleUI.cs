using System.Collections;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class SpeechBubbleUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private SpriteRenderer backgroundImage;
        [SerializeField] private TextMeshPro dialogueText;

        [Header("Typing Settings")]
        [SerializeField] private float charactersPerSecond = 40f;

        [Header("Layout Settings")]
        [SerializeField] private Vector2 padding = new Vector2(0.4f, 0.3f); // world units
        [SerializeField] private float resizeDuration = 0.25f;

        private Coroutine _typingCoroutine;
        private Coroutine _resizeCoroutine;

        public CanvasGroup CanvasGroup => canvasGroup;
        public float CharactersPerSecond => charactersPerSecond;

        public void Show(string speakerName, string lineText, Sprite portrait)
        {
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            if (_resizeCoroutine != null)
                StopCoroutine(_resizeCoroutine);

            _resizeCoroutine = StartCoroutine(ResizeAndType(lineText));
        }

        private IEnumerator ResizeAndType(string text)
        {
            // Step 1: Temporarily hide text while measuring
            string originalText = dialogueText.text;
            dialogueText.alpha = 0f;
            dialogueText.text = text;
            dialogueText.ForceMeshUpdate();

            // Get target width (only width)
            float targetWidth = GetTextWidthWithPadding();
            float initialWidth = backgroundImage.size.x;
            float height = backgroundImage.size.y; // keep existing height

            // Step 2: Animate width only
            float elapsed = 0f;
            while (elapsed < resizeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / resizeDuration);
                float newWidth = Mathf.Lerp(initialWidth, targetWidth, t);
                backgroundImage.size = new Vector2(newWidth, height);
                yield return null;
            }
            backgroundImage.size = new Vector2(targetWidth, height);

            // Step 3: Clear text and show again
            dialogueText.text = "";
            dialogueText.alpha = 1f;

            // Step 4: Type text
            _typingCoroutine = StartCoroutine(TypeText(text));
        }

        private IEnumerator TypeText(string text)
        {
            dialogueText.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                dialogueText.text = text.Substring(0, i + 1);
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }
        }

        private float GetTextWidthWithPadding()
        {
            dialogueText.ForceMeshUpdate();
            var bounds = dialogueText.textBounds.size;
            return bounds.x + padding.x;
        }
    }
}
