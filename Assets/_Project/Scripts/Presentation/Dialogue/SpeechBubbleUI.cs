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
        [SerializeField] private TextMeshPro speakerText;

        [Header("Typing Settings")]
        [SerializeField] private float charactersPerSecond = 40f;

        [Header("Layout Settings")]
        [SerializeField] private Vector2 padding = new Vector2(0.4f, 0.3f);
        [SerializeField] private float resizeDuration = 0.25f;
        [SerializeField] private float minWidth = 1.5f; 

        private Coroutine _typingCoroutine;
        private Coroutine _resizeCoroutine;
        private bool _isTyping;
        private string _fullText;
        
        public bool IsTyping => _isTyping;
        public CanvasGroup CanvasGroup => canvasGroup;
        public float CharactersPerSecond => charactersPerSecond;

        public void Show(string speakerName, string lineText, Sprite portrait)
        {
            // stop any current animations
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            if (_resizeCoroutine != null)
                StopCoroutine(_resizeCoroutine);

            _isTyping = false;

            speakerText.text = speakerName;
            speakerText.alpha = 1f;

            _resizeCoroutine = StartCoroutine(ResizeAndType(lineText));
        }

        private IEnumerator ResizeAndType(string text)
        {
            var rect = dialogueText.rectTransform;
            rect.sizeDelta = new Vector2(10f, rect.sizeDelta.y);

            dialogueText.alpha = 0f;
            dialogueText.text = text;
            dialogueText.ForceMeshUpdate();

            Vector2 targetSize = GetTotalTextSize();
            Vector2 initialSize = backgroundImage.size;
            float fixedHeight = initialSize.y;

            targetSize.x = Mathf.Max(targetSize.x, minWidth);
            targetSize.y = fixedHeight;

            float elapsed = 0f;
            while (elapsed < resizeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / resizeDuration);
                float newWidth = Mathf.Lerp(initialSize.x, targetSize.x, t);

                backgroundImage.size = new Vector2(newWidth, fixedHeight);
                UpdateTextBoxSize(new Vector2(newWidth, fixedHeight));
                yield return null;
            }

            backgroundImage.size = new Vector2(targetSize.x, fixedHeight);
            UpdateTextBoxSize(new Vector2(targetSize.x, fixedHeight));

            dialogueText.text = "";
            dialogueText.alpha = 1f;

            _fullText = text;
            _isTyping = true;               
            _typingCoroutine = StartCoroutine(TypeText(text));
        }
        
        private Vector2 GetTotalTextSize()
        {
            dialogueText.ForceMeshUpdate();
            Vector2 dialogueSize = dialogueText.textBounds.size;

            speakerText.ForceMeshUpdate();
            Vector2 speakerSize = speakerText.textBounds.size;

            float finalWidth = Mathf.Max(dialogueSize.x, speakerSize.x) + padding.x;

            float finalHeight = speakerSize.y + dialogueSize.y + padding.y;

            return new Vector2(finalWidth, finalHeight);
        }
        
        public void SkipTyping()
        {
            if (!_isTyping) return;

            _isTyping = false;

            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }

            dialogueText.text = _fullText;
        }

        private void UpdateTextBoxSize(Vector2 backgroundSize)
        {
            var rect = dialogueText.rectTransform;
            rect.sizeDelta = backgroundSize - padding;
        }

        private IEnumerator TypeText(string text)
        {
            dialogueText.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (!_isTyping)
                    yield break;

                dialogueText.text = text.Substring(0, i + 1);
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }

            _isTyping = false;
            _typingCoroutine = null;
        }

        private Vector2 GetTextSizeWithPadding()
        {
            dialogueText.ForceMeshUpdate();
            var bounds = dialogueText.textBounds.size;
            return new Vector2(bounds.x + padding.x, backgroundImage.size.y);
        }
    }
}
