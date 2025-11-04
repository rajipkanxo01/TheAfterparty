using System;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Tools.Tilemap;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Application.Player
{
    public class PlayerMovementManager
    {
        // Animating from first Vector2 to second Vector2
        public event Action<Vector3Int, Vector3Int, int> OnMoveStarted;
        public event Action<bool> ToggleCrawl;

        private Vector3Int currentGridPos;
        private Vector3Int targetGridPos;
        private bool isMoving;
        private bool isCrawling;

        // If the player is holding two directions at once, we will try to alternate between them
        // (unless they are against a wall)
        private bool lastMoveVertical;
        private Vector3Int facingDir;

        // Tilemap storage for collisions
        private MysteryTilemapHelper helper;
        private List<Tilemap> tilemapLayers;
        private int currentLayer;
        private int targetLayer;
        
        private GameStateService _gameStateService;
        private bool _canMove = true;
        
        public PlayerMovementManager(Vector3Int startGridPos, List<Tilemap> _tilemapLayers, int _currentLayer)
        {
            currentGridPos = startGridPos;
            targetGridPos = startGridPos;

            tilemapLayers = _tilemapLayers;
            helper = new MysteryTilemapHelper(tilemapLayers);
            currentLayer = _currentLayer;
            targetLayer = currentLayer;
        
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _gameStateService.OnStateChanged += HandleGameStateChanged;
        }
        
        private void HandleGameStateChanged(GameState newState)
        {
            // _canMove = newState == GameState.Normal || newState == GameState.Cutscene;
        }

        public void ProcessMoveInput(Vector3Int input)
        {
            if (input == Vector3Int.zero)
            {
                // TODO: maybe some extra processing to handle the two directions held down issue better?
                return;
            }
            
            
            // disable movement during dialogues, etc.
            /*if (!_canMove)
            {
                return;
            };*/
            
            if (isMoving) return;


            Vector3Int moveDirection = input;
            // Handle walls
            Vector3Int movePosX = currentGridPos + new Vector3Int(moveDirection.x, 0, 0);
            Vector3Int movePosY = currentGridPos + new Vector3Int(0, moveDirection.y, 0);

            if (moveDirection.x != 0 
                && (!helper.IsWalkable(movePosX, currentLayer) // There isn't a tile to step forwards onto
                    || (!isCrawling && helper.IsSolid(movePosX, currentLayer + 1)) // There is a layer above + theres a wall in front
                    || (isCrawling && !helper.CanCrawlUnder(movePosX, currentLayer + 1))))  
            {
                moveDirection.x = 0;
            }
            if (moveDirection.y != 0
                && (!helper.IsWalkable(movePosY, currentLayer) // There isn't a tile to step forwards onto
                    || (!isCrawling && helper.IsSolid(movePosY, currentLayer + 1)) // There is a layer above + theres a wall in front
                    || (isCrawling && !helper.CanCrawlUnder(movePosY, currentLayer + 1))))  
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

            // disable movement during dialogues, cutscenes, etc.
            if (!_canMove)
            {
                return;
            };

            // Position we are hoping to jump to
            Vector3Int checkGridPos = currentGridPos + facingDir;

            // If the layer above in the direction you is jumpable
            // AND the layer two above isnt solid
            if (helper.IsWalkable(checkGridPos, currentLayer + 1) && helper.CanJumpOnto(checkGridPos, currentLayer + 1) 
                && !helper.IsSolid(checkGridPos, currentLayer + 2))
            {
                // Jump one space up
                targetLayer = currentLayer + 1;
                targetGridPos = checkGridPos;
                OnMoveStarted?.Invoke(currentGridPos, targetGridPos, targetLayer);

            } // If the layer above you is not blocking the way AND theres a hole in front
            else if (!helper.IsSolid(checkGridPos, currentLayer) && !helper.IsSolid(checkGridPos, currentLayer + 1))
            {
                // Fall down as far as we can (unless we hit nothing)
                int layer = currentLayer - 1;
                while (layer >= 0)
                {
                    if (helper.IsSolid(checkGridPos, layer)) break;
                    layer -= 1;
                }

                if (layer >= 0 && helper.CanJumpOnto(checkGridPos, layer) && helper.IsWalkable(checkGridPos, layer))
                {
                    // Jump down
                    targetLayer = layer;
                    targetGridPos = checkGridPos;
                    OnMoveStarted?.Invoke(currentGridPos, targetGridPos, targetLayer);
                }
            }
        }

        public void ProcessCrawlInput()
        {
            // disable movement during dialogues, cutscenes, etc.
            if (!_canMove)
            {
                return;
            };

            if (isCrawling)
            {
                if (helper.IsSolid(currentGridPos, currentLayer + 1) || helper.IsSolid(targetGridPos, targetLayer + 1)) return;

                isCrawling = false;
                ToggleCrawl?.Invoke(isCrawling);
            }
            else
            {
                // Can always start crawling (TODO: should this still be possible while jumping??)
                isCrawling = true;
                ToggleCrawl?.Invoke(isCrawling);
            } 
        }

        // Called after the movement has been animated
        public void DoneMoving()
        {
            currentGridPos = targetGridPos;
            currentLayer = targetLayer;
            isMoving = false;
        }        
    }
}
