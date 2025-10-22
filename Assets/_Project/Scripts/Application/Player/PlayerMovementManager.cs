using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    public class PlayerMovementManager
    {
        // Animating from first Vector2 to second Vector2
        public event Action<Vector2Int, Vector2Int> OnMoveStarted;

        private Vector2Int currentGridPos;
        private Vector2Int targetGridPos;
        private bool isMoving;

        // If the player is holding two directions at once, we will try to alternate between them
        // (unless they are against a wall)
        private bool lastMoveVertical;

        public PlayerMovementManager(Vector2Int startGridPos)
        {
            currentGridPos = startGridPos;
            targetGridPos = startGridPos;
        }

        public void ProcessMoveInput(Vector2Int input)
        {
            if (input == Vector2Int.zero)
            {
                // TODO: maybe some extra processing to handle the two directions held down issue better?
                return;
            }
            if (isMoving) return;
            

            Vector2Int moveDirection = input;

            // Pressing two directions at once, need to choose one
            if (input.x != 0 && input.y != 0)
            {
                if (lastMoveVertical) moveDirection = new Vector2Int(input.x, 0);
                else moveDirection = new Vector2Int(0, input.y);
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
