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
        public event Action<Vector3Int, Vector3Int, int> OnMoveStarted;

        private Vector3Int currentGridPos;
        private Vector3Int targetGridPos;
        private bool isMoving;

        // If the player is holding two directions at once, we will try to alternate between them
        // (unless they are against a wall)
        private bool lastMoveVertical;
        private Vector3Int facingDir;

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
            if (moveDirection.x != 0 
                && (!tilemapLayers[currentLayer].HasTile(currentGridPos + new Vector3Int(moveDirection.x, 0, 0)) // There isn't a tile to step forwards onto
                    || (currentLayer + 1 < tilemapLayers.Count // There is a layer above + theres a wall in front
                        && tilemapLayers[currentLayer + 1].HasTile(currentGridPos + new Vector3Int(moveDirection.x, 0, 0)))))
            {
                moveDirection.x = 0;
            }
            if (moveDirection.y != 0
                && (!tilemapLayers[currentLayer].HasTile(currentGridPos + new Vector3Int(0, moveDirection.y, 0)) // There isn't a tile to step forwards onto
                    || (currentLayer + 1 < tilemapLayers.Count // There is a layer above + theres a wall in front
                        && tilemapLayers[currentLayer + 1].HasTile(currentGridPos + new Vector3Int(0, moveDirection.y, 0)))))
            {
                moveDirection.y = 0;
            }

            // Pressing two directions at once, need to choose one
            if (moveDirection.x != 0 && moveDirection.y != 0)
            {
                if (lastMoveVertical) moveDirection = new Vector3Int(moveDirection.x, 0);
                else moveDirection = new Vector3Int(0, moveDirection.y);
            }

            // Is running into a wall -> we should still set the sprite to face the direction
            if (moveDirection.x == 0 && moveDirection.y == 0)
            {
                // TODO: currently will just default to y input for facing
                // not a common occurence for them to be pressing two directions into a wall
                if (input.y != 0)
                {
                    facingDir = new Vector3Int(0, input.y, 0);
                }
                else if (input.x != 0)
                {
                    facingDir = new Vector3Int(input.x, 0, 0);
                }
            }
            else
            {
                facingDir = moveDirection;

                lastMoveVertical = moveDirection.y != 0;
                targetGridPos = currentGridPos + moveDirection;
                isMoving = true;

                OnMoveStarted?.Invoke(currentGridPos, targetGridPos, currentLayer);
            }
        }

        public void ProcessJumpInput()
        {
            if (isMoving) return;

            // Position we are hoping to jump to
            Vector3Int checkGridPos = currentGridPos + facingDir;

            // If the layer above in the direction you are facing has a tile
            // AND the layer two above doesnt exist or has an open space 
            if (currentLayer + 1 < tilemapLayers.Count && tilemapLayers[currentLayer + 1].HasTile(checkGridPos)
               && (currentLayer + 2 >= tilemapLayers.Count || !tilemapLayers[currentLayer + 2].HasTile(checkGridPos)))
            {
                // Jump one space up
                currentLayer = currentLayer + 1;
                targetGridPos = checkGridPos;
                OnMoveStarted?.Invoke(currentGridPos, targetGridPos, currentLayer);

            } // If the layer above you is not blocking the way 
            else if (!tilemapLayers[currentLayer].HasTile(checkGridPos)
                     && (currentLayer + 1 >= tilemapLayers.Count || !tilemapLayers[currentLayer + 1].HasTile(checkGridPos)))
            {
                // Fall down as far as we can (unless we hit nothing)
                int layer = currentLayer - 1;
                while (layer >= 0)
                {
                    if (tilemapLayers[layer].HasTile(checkGridPos)) break;
                    layer -= 1;
                }

                if (layer >= 0)
                {
                    // Jump down
                    currentLayer = layer;
                    targetGridPos = checkGridPos;
                    OnMoveStarted?.Invoke(currentGridPos, targetGridPos, layer);
                }
            }
        }

        // Called after the movement has been animated
        public void DoneMoving()
        {
            currentGridPos = targetGridPos;
            isMoving = false;
        }
    }
}
