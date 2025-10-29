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
        
        public CanvasGroup CanvasGroup => canvasGroup;
        public Image PortraitImage => portraitImage;
        public TextMeshProUGUI DialogueText => dialogueText;
        public TextMeshProUGUI SpeakerText => speakerText;
    }
}