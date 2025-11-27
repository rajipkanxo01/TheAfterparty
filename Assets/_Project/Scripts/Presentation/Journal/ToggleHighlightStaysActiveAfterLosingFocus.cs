using _Project.Scripts.Application.Events;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal
{
    public class ToggleHighlightStaysActiveAfterLosingFocus : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image imageToKeepFocusActive;
        [SerializeField] private int tabIndex;

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }

        private void Awake()
        {
            UIEvents.OnJournalTabChanged += HandleJournalTabChanged;
        }
        
        private void OnDestroy()
        {
            UIEvents.OnJournalTabChanged -= HandleJournalTabChanged;
        }
        
        private void HandleJournalTabChanged(int newTabIndex)
        {
            imageToKeepFocusActive.color = newTabIndex == tabIndex ? toggle.colors.selectedColor : toggle.colors.normalColor;
        }
    }
}