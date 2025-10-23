using _Project.Scripts.Application.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Presentation.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector2Int startGridPos;

        private PlayerMovementManager movementManager;
        private Vector2 currentPos;
        private Vector2 targetPos;
        private float moveTimer;
        private bool isMoving;

        private Vector2Int moveInput;

        private void Awake()
        {
            movementManager = new PlayerMovementManager(startGridPos);

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
            moveInput = new Vector2Int(Mathf.RoundToInt(floatInput.x), Mathf.RoundToInt(floatInput.y));
        }

        private void StartMoveAnimation(Vector2Int from, Vector2Int to)
        {
            currentPos = GridToWorld(from);
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

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new Vector2(
                (gridPos.x + gridPos.y) * 0.5f, // NOTE: can adjust this depending on tilemap size
                (gridPos.y - gridPos.x) * 0.25f
            );
        }
    }
}