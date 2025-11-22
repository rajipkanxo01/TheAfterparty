using _Project.Scripts.Application.Events;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject menu;

        private void Awake()
        {
            menu.SetActive(false);
        }

        private void OnEnable()
        {
            UIEvents.OnJournalOpen += Show;
            UIEvents.OnJournalClose += Hide;
        }

        private void OnDisable()
        {
            UIEvents.OnJournalOpen -= Show;
            UIEvents.OnJournalClose -= Hide;
        }

        private void Show()
        {
            menu.SetActive(true);
        }

        private void Hide()
        {
            menu.SetActive(false);
        }
    }
}