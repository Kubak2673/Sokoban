using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveDistance = 1.0f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Rigidbody2D rb;
    public LayerMask obstacleLayer;
    public Sprite upSprite;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    private StepCounter stepCounter;
    private Stack<GameState> history = new Stack<GameState>(); // Stack to save history

    void Start()
    {
        stepCounter = FindObjectOfType<StepCounter>(); // Find the StepCounter script in the scene
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        targetPosition = rb.position;
        SaveState(); // Save the initial state at the start of the game
    }

    void Update()
    {
        if (!isMoving)
        {
            // Check for undo input (Z key or RB on gamepad)
            if (Input.GetKeyDown(KeyCode.Z) || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame))
            {
                UndoMove();
                return;
            }

            Vector2 moveDirection = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Vector2.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Vector2.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Vector2.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Vector2.right;

            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.up.wasPressedThisFrame) moveDirection = Vector2.up;
                if (Gamepad.current.leftStick.down.wasPressedThisFrame) moveDirection = Vector2.down;
                if (Gamepad.current.leftStick.left.wasPressedThisFrame) moveDirection = Vector2.left;
                if (Gamepad.current.leftStick.right.wasPressedThisFrame) moveDirection = Vector2.right;
            }

            if (moveDirection != Vector2.zero)
            {
                StartCoroutine(MovePlayer(moveDirection));
            }
        }
    }

    private System.Collections.IEnumerator MovePlayer(Vector2 direction)
    {
        SaveState(); // Save the state before moving

        isMoving = true;
        targetPosition = rb.position + direction * moveDistance;

        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        
        // If path is clear and no box or only the goal is at the target
        if (IsPathClear(targetPosition) && (hitCollider == null || hitCollider.CompareTag("Goal")))
        {
            rb.MovePosition(targetPosition);
            stepCounter.IncrementStepCounter(); // Increment step when moving player
        }
        else if (hitCollider != null && hitCollider.CompareTag("Box"))
        {
            BoxMovement box = hitCollider.GetComponent<BoxMovement>();
            Vector2 boxTargetPosition = (Vector2)hitCollider.transform.position + direction * moveDistance;
            if (box != null && box.IsPathClear(boxTargetPosition, true))
            {
                box.MoveBox(boxTargetPosition); // Move the box
                rb.MovePosition(targetPosition); // Move the player
                stepCounter.IncrementStepCounter(); // Increment step when pushing a box
            }
        }

        yield return new WaitForFixedUpdate();
        isMoving = false;
    }

    private void SaveState()
    {
        // Save the current state
        List<BoxState> boxStates = new List<BoxState>();
        foreach (var box in FindObjectsOfType<BoxMovement>())
        {
            boxStates.Add(new BoxState(box.transform.position));
        }

        history.Push(new GameState(rb.position, boxStates)); // Save the current state
    }

    private void UndoMove()
    {
        if (history.Count > 1)
        {
            GameState lastState = history.Pop();
            rb.position = lastState.PlayerPosition;

            var boxes = FindObjectsOfType<BoxMovement>();
            for (int i = 0; i < lastState.BoxStates.Count && i < boxes.Length; i++)
            {
                boxes[i].transform.position = lastState.BoxStates[i].Position;
            }

            // Inform step counter that an undo occurred
            stepCounter.UndoMove();
        }
    }

    private bool IsPathClear(Vector2 targetPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        return hitCollider == null || (!hitCollider.CompareTag("Wall") && !hitCollider.CompareTag("Box"));
    }
}
