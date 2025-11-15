using System;
using UnityEngine;

namespace _Project.Scripts.Application.Core
{
    public enum GameState
    {
        Normal,
        Dialogue,
        Cutscene,
        Journal,
        Paused,
        FragmentPlaying
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
            
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }

        public bool IsState(GameState state)
        {
            return _currentState == state;
        }

        public bool CanMove()
        {
            if (_currentState == GameState.Normal || _currentState == GameState.Cutscene || _currentState == GameState.FragmentPlaying)
            {
                return true;
            }
            return false;
        }
        
    }
}