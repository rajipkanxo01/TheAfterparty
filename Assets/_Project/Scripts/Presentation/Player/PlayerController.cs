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
        [SerializeField] private float crawlSpeed;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private PlayerMovementManager movementManager;
        // private TileOpacityManager tileOpacityManager;
        private Vector2 currentPos;
        private Vector2 targetPos;
        private float moveTimer;
        private bool isMoving;
        private bool isCrawling;

        private Vector3Int moveInput;

        [Header("Tilemaps")]
        [Tooltip("Please set to the layer the player is initially standing on")]
        [SerializeField] private int groundLayer;
        [SerializeField] private List<Tilemap> tilemapLayers;
        private int currentLayer;

        // [SerializeField] private Material tilemapMat;

        private void Start()
        {
            currentLayer = groundLayer;

            Vector3Int startGridPos3D = tilemapLayers[currentLayer].WorldToCell(transform.position);
            Vector3Int startGridPos = new Vector3Int(startGridPos3D.x, startGridPos3D.y);
            movementManager = new PlayerMovementManager(startGridPos, tilemapLayers, currentLayer);

            movementManager.OnMoveStarted += StartMoveAnimation;
            movementManager.ToggleCrawl += SetCrawl;

            currentPos = GridToWorld(startGridPos, currentLayer);
            targetPos = currentPos;
            transform.position = currentPos;

            spriteRenderer.sortingOrder = currentLayer + 1;

            // Manages the materials to hide tiles based on player position
            // tileOpacityManager = new TileOpacityManager(tilemapLayers, currentLayer, tilemapMat);
            // tileOpacityManager.UpdatePosition(transform.position);
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
        public void JumpInput(InputAction.CallbackContext context)
        {
            if(context.performed) movementManager.ProcessJumpInput();
        }
        public void CrawlInput(InputAction.CallbackContext context)
        {
            if(context.performed) movementManager.ProcessCrawlInput();
        }

        private void StartMoveAnimation(Vector3Int from, Vector3Int to, int toLayer)
        {
            currentPos = GridToWorld(from, currentLayer);
            targetPos = GridToWorld(to, toLayer);
            if(currentLayer < toLayer) spriteRenderer.sortingOrder = toLayer + 1;
            currentLayer = toLayer;


            moveTimer = 0f;
            isMoving = true;
        }

        private void UpdateMoveAnimation()
        {
            if (!isMoving) return;

            moveTimer += (isCrawling ? crawlSpeed : moveSpeed) * Time.deltaTime;

            if (moveTimer >= 1f)
            {
                moveTimer = 1f;
                isMoving = false;
                spriteRenderer.sortingOrder = currentLayer + 1;
                movementManager.DoneMoving();
            }

            transform.position = Vector2.Lerp(currentPos, targetPos, moveTimer);
            // tileOpacityManager.UpdatePosition(transform.position);
            // TODO: have to change the sorting layer here
        }

        private void SetCrawl(bool crawling)
        {
            isCrawling = crawling;

            // TODO: remove this when we have actual sprites for the cat
            if (isCrawling) spriteRenderer.transform.localScale = new Vector2(1f, 0.5f);
            else spriteRenderer.transform.localScale = new Vector2(1f, 1f);
                
        }

        private Vector2 GridToWorld(Vector3Int gridPos, int layer)
        {
            if(layer >= tilemapLayers.Count) Debug.LogError(layer);

            return tilemapLayers[layer].CellToWorld(gridPos);
        }
    }
}