using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1.0f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Rigidbody2D rb;
    private LevelLoader levelLoader;  // Reference to LevelLoader
    public LayerMask obstacleLayer;

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
        // If the level is completed, do not allow movement
        if (levelLoader != null && levelLoader.IsLevelCompleted())
        {
            return;  // If the level is completed, stop further execution in Update
        }

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
        }
    }

    // Coroutine to handle movement in steps
    IEnumerator MovePlayer(Vector2 direction)
    {
        isMoving = true;  // Prevent further movement during this process
        targetPosition = rb.position + direction * moveDistance;  // Set target position

        // Check if the target position is clear of obstacles
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        if (IsPathClear(targetPosition) && (hitCollider == null || hitCollider.CompareTag("Goal")))
        {
            rb.MovePosition(targetPosition);  // Move to the target position
        }
        else if (hitCollider != null && hitCollider.CompareTag("Box"))
        {
            BoxMovement box = hitCollider.GetComponent<BoxMovement>();
            Vector2 boxTargetPosition = (Vector2)hitCollider.transform.position + direction * moveDistance;
            if (box != null && box.IsPathClear(boxTargetPosition, true))
            {
                box.MoveBox(boxTargetPosition);  // Move the box
                rb.MovePosition(targetPosition);  // Move the player to the target position
            }
        }

        // Wait for the next fixed update cycle
        yield return new WaitForFixedUpdate();
        isMoving = false;  // Allow movement again
    }

    // Method to check if the target path is clear
    bool IsPathClear(Vector2 targetPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        return hitCollider == null || (!hitCollider.CompareTag("Wall") && !hitCollider.CompareTag("Box"));
    }
}
