using System;
using UnityEngine;

namespace _Project.Scripts.Application.Core
{
    public enum GameState
    {
        Normal,
        Dialogue,
        Cutscene,
        Paused
    }

    public class GameStateService
    {
        private GameState _currentState = GameState.Normal;
        public GameState CurrentState => _currentState;

        public event Action<GameState> OnStateChanged;

        public void SetState(GameState newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            Debug.Log($"GameState changed: {_currentState} → {newState}");

            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }

        public bool IsState(GameState state)
        {
            return _currentState == state;
        }
    }
}