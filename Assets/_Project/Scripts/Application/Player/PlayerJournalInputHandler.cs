using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Application.Player
{
    public class PlayerJournalInputHandler : MonoBehaviour
    {
        private bool _isOpen;

        private GameStateService _gameState;

        private void Start()
        {
            _gameState = ServiceLocater.GetService<GameStateService>();
        }

        public void OnToggleJournal(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            // block during cutscene/dialogue/transition
            if (_gameState.IsState(GameState.Dialogue) ||
                _gameState.IsState(GameState.Cutscene) ||
                _gameState.IsState(GameState.Transition))
            {
                return;
            }

            if (_isOpen)
            {
                _isOpen = false;
                _gameState.SetState(GameState.Normal);
                UIEvents.RaiseJournalClose();
            }
            else
            {
                _isOpen = true;
                _gameState.SetState(GameState.Journal);
                UIEvents.RaiseJournalOpen();
            }
        }
    }
}