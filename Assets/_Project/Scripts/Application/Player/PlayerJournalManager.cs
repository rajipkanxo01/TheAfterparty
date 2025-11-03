using System;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Presentation.Journal;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    public class PlayerJournalManager
    {
        public event Action<bool> ToggleJournal;

        private GameStateService _gameStateService;
        private PlayerProfile _playerProfile;
        private JournalManager journalManager;

        private int _numShownClues = 0;

        public PlayerJournalManager(JournalManager _journalManager)
        {
            journalManager = _journalManager;

            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        public void ProcessJournalInput()
        {
            if (!_gameStateService.IsState(GameState.Normal) && !_gameStateService.IsState(GameState.Journal)) return;

            if (_gameStateService.IsState(GameState.Normal))
            {
                // Open journal
                _gameStateService.SetState(GameState.Journal);

                // Update journal contents
                if(_playerProfile.GetDiscoveredClues().Count > _numShownClues)
                {
                    // Could show an animation here later
                    journalManager.RecreateClues(_playerProfile.GetDiscoveredClues());
                    _numShownClues = _playerProfile.GetDiscoveredClues().Count;
                } 

                ToggleJournal?.Invoke(true);
            }
            else
            {
                // Close journal
                _gameStateService.SetState(GameState.Normal);
                ToggleJournal?.Invoke(false);
            }
        }
    }
}
