using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerControllerContinuous : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float crawlSpeed;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;

        private bool canMove = true;
        private bool isCrawling;

        private Vector2 moveInput;

        private PauseMenu pauseMenu;
        private GameStateService _gameStateService;

        private void Start()
        {
            if(!(GameObject.Find("PauseMenu")?.TryGetComponent<PauseMenu>(out pauseMenu) ?? false)){
                Debug.LogError("Couldn't find pause menu");
            }

            _gameStateService = ServiceLocater.GetService<GameStateService>();
            _gameStateService.OnStateChanged += HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            canMove = _gameStateService.CanMove();
            if (!canMove)
            {
                moveInput = Vector2.zero;
            }
        }

        private void Update()
        {
            // Needs to be in update as if player somehow gets hit off trajectory they can veer in wrong direction
            rb.linearVelocity = (isCrawling ? crawlSpeed : moveSpeed) * moveInput;
        }

        public void MoveInput(InputAction.CallbackContext context)
        {
            if (!canMove)
            {
                moveInput = Vector2.zero;
                return;
            }
            
            Vector2 input = context.ReadValue<Vector2>();
            if (input.x != 0 && input.y != 0)
            {
                // Going along a diagonal, shift to be along tilemap 
                moveInput = new Vector2(Mathf.Sign(input.x) * 1f, Mathf.Sign(input.y) * 0.5f).normalized;
            }
            else moveInput = input;
        }

        public void CrawlInput(InputAction.CallbackContext context)
        {
            if (context.performed) SetCrawl(!isCrawling);
        }

        public void PauseInput(InputAction.CallbackContext context)
        {
            if (context.performed && pauseMenu) pauseMenu.TogglePause();
        }

        private void SetCrawl(bool crawling)
        {
            isCrawling = crawling;

            // TODO: remove this when we have actual sprites for the cat
            if (isCrawling) spriteRenderer.transform.localScale = new Vector2(1f, 0.5f);
            else spriteRenderer.transform.localScale = new Vector2(1f, 1f);
                
        }
    }
}