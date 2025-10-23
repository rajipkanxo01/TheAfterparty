using System.Collections.Generic;
using _Project.Scripts.Application.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;

        private PlayerMovementManager movementManager;
        private Vector2 currentPos;
        private Vector2 targetPos;
        private float moveTimer;
        private bool isMoving;

        private Vector3Int moveInput;

        [Header("Tilemaps")]
        [Tooltip("Please set to the layer the player is initially standing on")]
        [SerializeField] private int groundLayer;
        [SerializeField] private List<Tilemap> tilemapLayers;
        private int currentLayer;

        private void Awake()
        {
            currentLayer = groundLayer;

            Vector3Int startGridPos3D = tilemapLayers[currentLayer].WorldToCell(transform.position);
            Vector3Int startGridPos = new Vector3Int(startGridPos3D.x, startGridPos3D.y);
            movementManager = new PlayerMovementManager(startGridPos, tilemapLayers, currentLayer);

            movementManager.OnMoveStarted += StartMoveAnimation;

            currentPos = GridToWorld(startGridPos);
            targetPos = currentPos;
            transform.position = currentPos;
        }

        private void Update()
        {
            movementManager.ProcessMoveInput(moveInput);
            UpdateMoveAnimation();
        }

        public void MoveInput(InputAction.CallbackContext context)
        {
            Vector2 floatInput = context.ReadValue<Vector2>();
            moveInput = new Vector3Int(Mathf.RoundToInt(floatInput.y), -Mathf.RoundToInt(floatInput.x));
        }

        private void StartMoveAnimation(Vector3Int from, Vector3Int to)
        {
            currentPos = tilemapLayers[0].CellToWorld(from);
            targetPos = GridToWorld(to);
            moveTimer = 0f;
            isMoving = true;
        }

        private void UpdateMoveAnimation()
        {
            if (!isMoving) return;

            moveTimer += moveSpeed * Time.deltaTime;

            if (moveTimer >= 1f)
            {
                moveTimer = 1f;
                isMoving = false;
                movementManager.DoneMoving();
            }

            transform.position = Vector2.Lerp(currentPos, targetPos, moveTimer);
            // TODO: have to change the sorting layer here
        }

        private Vector2 GridToWorld(Vector3Int gridPos)
        {
            return tilemapLayers[currentLayer].CellToWorld(gridPos);
        }
    }
}