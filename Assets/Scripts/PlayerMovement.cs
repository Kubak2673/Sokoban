using System.Collections.Generic;  // Include for List and Dictionary
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1.0f;   // Distance the player moves
    private Vector2 targetPosition;      // The target position for the player
    private bool isMoving = false;       // Check if the player is currently moving
    private Rigidbody2D rb;              // Reference to the player's Rigidbody2D
    private LevelLoader levelLoader;     // Reference to LevelLoader
    public LayerMask obstacleLayer;      // Layer mask for obstacles

    // To store the last action's state for undo functionality
    private Stack<UndoState> undoStack = new Stack<UndoState>();

    private int stepCount = 0;           // Step counter

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = rb.position;

        // Find the LevelLoader object in the scene
        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
        }
    }

    void Update()
    {
        // Only allow movement if the player is not already moving
        if (!isMoving)
        {
            Vector2 moveDirection = Vector2.zero;

            // Keyboard input for movement
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Vector2.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Vector2.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Vector2.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Vector2.right;

            // Gamepad input for movement
            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.up.wasPressedThisFrame) moveDirection = Vector2.up;
                if (Gamepad.current.leftStick.down.wasPressedThisFrame) moveDirection = Vector2.down;
                if (Gamepad.current.leftStick.left.wasPressedThisFrame) moveDirection = Vector2.left;
                if (Gamepad.current.leftStick.right.wasPressedThisFrame) moveDirection = Vector2.right;
            }

            // Start movement coroutine if a direction is chosen
            if (moveDirection != Vector2.zero)
            {
                StartCoroutine(MovePlayer(moveDirection));
            }

            // Check for undo command (e.g., pressing Z key)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoMove();
            }
        }
    }

    // Coroutine to handle movement in steps
    private System.Collections.IEnumerator MovePlayer(Vector2 direction)
    {
        // Save the current state for undo functionality
        SaveCurrentState();
        isMoving = true;  // Prevent further movement during this process
        targetPosition = rb.position + direction * moveDistance;  // Set target position

        // Check if the target position is clear of obstacles
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        if (IsPathClear(targetPosition) && (hitCollider == null || hitCollider.CompareTag("Goal")))
        {
            rb.MovePosition(targetPosition);  // Move to the target position
            IncrementStepCount();  // Increment step count when player moves
        }
        else if (hitCollider != null && hitCollider.CompareTag("Box"))
        {
            BoxMovement box = hitCollider.GetComponent<BoxMovement>();
            Vector2 boxTargetPosition = (Vector2)hitCollider.transform.position + direction * moveDistance;
            if (box != null && box.IsPathClear(boxTargetPosition, true))
            {
                box.MoveBox(boxTargetPosition);  // Move the box
                rb.MovePosition(targetPosition);  // Move the player to the target position
                IncrementStepCount();  // Increment step count when player moves
            }
        }

        // Wait for the next fixed update cycle
        yield return new WaitForFixedUpdate();
        isMoving = false;  // Allow movement again
    }

    // Method to save the current state for undo functionality
    private void SaveCurrentState()
    {
        UndoState currentState = new UndoState
        {
            PlayerPosition = rb.position,
            BoxPositions = new Dictionary<BoxMovement, Vector2>()
        };

        // Get all boxes and save their positions
        BoxMovement[] boxes = FindObjectsOfType<BoxMovement>();
        foreach (BoxMovement box in boxes)
        {
            currentState.BoxPositions[box] = box.transform.position;
        }

        // Push the current state onto the stack
        undoStack.Push(currentState);
    }

    // Undo the last move
    private void UndoMove()
    {
        if (undoStack.Count > 0)
        {
            UndoState lastUndoState = undoStack.Pop();  // Get the last state from the stack
            rb.MovePosition(lastUndoState.PlayerPosition);  // Restore player position

            // Restore box positions
            foreach (var entry in lastUndoState.BoxPositions)
            {
                entry.Key.transform.position = entry.Value;  // Restore box position
            }
        }
    }

    // Method to check if the target path is clear
    private bool IsPathClear(Vector2 targetPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        return hitCollider == null || (!hitCollider.CompareTag("Wall") && !hitCollider.CompareTag("Box"));
    }

    // Increment the step count
   private void IncrementStepCount()
{
    stepCount++;
    levelLoader?.UpdateStepCount(stepCount); // Update step count in LevelLoader
    LevelTimer levelTimer = GetComponent<LevelTimer>();
    if (levelTimer != null)
    {
        levelTimer.IncrementStepCount(); // Call to increment step count in LevelTimer
    }
}

    // Struct to hold the state for undo functionality
    private struct UndoState
    {
        public Vector2 PlayerPosition;
        public Dictionary<BoxMovement, Vector2> BoxPositions;  // Store positions of boxes
    }
}
