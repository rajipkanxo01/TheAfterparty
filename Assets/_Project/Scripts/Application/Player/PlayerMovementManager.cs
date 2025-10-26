using System;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Application.Player
{
    public class PlayerMovementManager
    {
        // Animating from first Vector2 to second Vector2
        public event Action<Vector3Int, Vector3Int> OnMoveStarted;

        private Vector3Int currentGridPos;
        private Vector3Int targetGridPos;
        private bool isMoving;

        // If the player is holding two directions at once, we will try to alternate between them
        // (unless they are against a wall)
        private bool lastMoveVertical;

        // Tilemap storage for collisions
        private List<Tilemap> tilemapLayers;
        private int currentLayer;
        
        private GameStateService _gameStateService;
        private bool _canMove = true;
        
        public PlayerMovementManager(Vector3Int startGridPos, List<Tilemap> _tilemapLayers, int _currentLayer)
        {
            currentGridPos = startGridPos;
            targetGridPos = startGridPos;

            tilemapLayers = _tilemapLayers;
            currentLayer = _currentLayer;
            
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _gameStateService.OnStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnStateChanged -= HandleGameStateChanged;
            }
        }
        
        private void HandleGameStateChanged(GameState newState)
        {
            _canMove = newState == GameState.Normal;
            Debug.Log("HandleGameStateChanged : canMove = " + _canMove);
        }

        public void ProcessMoveInput(Vector3Int input)
        {
            if (input == Vector3Int.zero)
            {
                // TODO: maybe some extra processing to handle the two directions held down issue better?
                return;
            }
            
            
            // disable movement during dialogues, cutscenes, etc.
            if (!_canMove)
            {
                Debug.Log("PlayerMovementManager: canMove is false");
                return;
            };
            
            if (isMoving) return;


            Vector3Int moveDirection = input;
            // Handle walls
            if (moveDirection.x != 0 && currentLayer < tilemapLayers.Count
                && tilemapLayers[currentLayer + 1].HasTile(currentGridPos + new Vector3Int(moveDirection.x, 0, 0)))
            {
                moveDirection.x = 0;
            }
            if (moveDirection.y != 0 && currentLayer < tilemapLayers.Count
                && tilemapLayers[currentLayer + 1].HasTile(currentGridPos + new Vector3Int(0, moveDirection.y, 0)))
            {
                moveDirection.y = 0;
            }
            
            // Pressing two directions at once, need to choose one
            if (moveDirection.x != 0 && moveDirection.y != 0)
            {
                if (lastMoveVertical) moveDirection = new Vector3Int(moveDirection.x, 0);
                else moveDirection = new Vector3Int(0, moveDirection.y);
            }

            lastMoveVertical = moveDirection.y != 0;
            targetGridPos = currentGridPos + moveDirection;
            isMoving = true;

            OnMoveStarted?.Invoke(currentGridPos, targetGridPos);
        }

        // Called after the movement has been animated
        public void DoneMoving()
        {
            currentGridPos = targetGridPos;
            isMoving = false;
        }
    }
}
