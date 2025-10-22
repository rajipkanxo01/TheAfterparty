using _Project.Scripts.Application.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2Int startGridPos;

    private PlayerMovementManager movementManager;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private float moveTimer;
    private bool isMoving;

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
        HandleInput();
        UpdateMoveAnimation();
    }

    private void HandleInput()
    {
        // TODO: THIS IS SLAPPED TOGETHER, can improve with new input system
        Vector2Int input = new Vector2Int(0, 0);
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
        }

        movementManager.ProcessMoveInput(input);

        // TODO: can add interaction with items + jumping here later
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
