using _Project.Scripts.Presentation.Journal.MemoryTab;
using _Project.Scripts.Presentation.Journal.NotesTab;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Journal
{
    public class JournalNavigationInput : MonoBehaviour
    {
        [SerializeField] private JournalMenu journalMenu;
        private MemoryPageController _memoryPage;
        private NotesPageController _notesPage;

        private Vector2 _navigateInput;
        private bool _journalOpen = false;

        private void Awake()
        {
            _memoryPage = GetComponentInChildren<MemoryPageController>(true);
            _notesPage = GetComponentInChildren<NotesPageController>(true);
        }

        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            _navigateInput = ctx.ReadValue<Vector2>();
        }

        public void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (journalMenu.IsMemoryTabActive)
            {
                _memoryPage.SelectCurrent();
            }
            else if (journalMenu.IsNotesTabActive)
            {
                _notesPage.SelectCurrent();
            }
        }

        public void OnNextTab(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                journalMenu.SwitchToNextTab();
            }
        }

        public void OnPreviousTab(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                journalMenu.SwitchToPreviousTab();
            }
        }

        private void Update()
        {
            if (_navigateInput.y > 0.5f) Move(-1);
            if (_navigateInput.y < -0.5f) Move(+1);
        }

        private void Move(int direction)
        {
            if (journalMenu.IsMemoryTabActive)
            {
                _memoryPage.MoveSelection(direction);
            }
            else if (journalMenu.IsNotesTabActive)
            {
                _notesPage.MoveSelection(direction);
            }
        }
    }
}