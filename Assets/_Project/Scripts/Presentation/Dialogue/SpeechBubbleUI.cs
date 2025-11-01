using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class SpeechBubbleUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image portraitImage;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI speakerText;
        
        [Header("Typing Settings")]
        [SerializeField] private float charactersPerSecond = 40f;
        
        public CanvasGroup CanvasGroup => canvasGroup;
        public Image PortraitImage => portraitImage;
        public TextMeshProUGUI DialogueText => dialogueText;
        public TextMeshProUGUI SpeakerText => speakerText;
        
        private Coroutine _typingCoroutine;

        public void Show(string speakerName, string lineText, Sprite portrait)
        {
            speakerText.text = speakerName;
            portraitImage.sprite = portrait;
            
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }
            
            _typingCoroutine = StartCoroutine(TypeText(lineText));
        }

        private IEnumerator TypeText(string text)
        {
            dialogueText.text = "";
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }
        }
        
        
    }
}