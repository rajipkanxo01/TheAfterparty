using _Project.Scripts.Application.Core;
using UnityEngine;

namespace _Project.Scripts.Presentation.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        private GameStateService _gameStateService;
        private GameState prevState = GameState.Normal;

        private void Start()
        {
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _gameStateService.OnStateChanged += HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state != GameState.Paused) prevState = state;
        }

        public void SetPause(bool pause)
        {
            if (pause) _gameStateService.SetState(GameState.Paused);
            else _gameStateService.SetState(prevState);
            pausePanel.SetActive(pause);
        }

        public void TogglePause()
        {
            bool pause = !pausePanel.activeSelf;
            SetPause(pause);
        }
    }
}
