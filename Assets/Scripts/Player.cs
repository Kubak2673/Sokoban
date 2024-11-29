using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    private Stack<GameState> history = new Stack<GameState>(); 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        targetPosition = rb.position;
        stepCounter = FindObjectOfType<StepCounter>(); 
        SaveState(); 
    }
    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            GoToMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
        if (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame)
        {
            UndoMove();
            return;
        }
        if (!isMoving)
        {
            Vector2 moveDirection = Vector2.zero;
            if (Input.GetKeyDown(KeyCode.Z) || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame))
            {
                UndoMove();
                return;
            }
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
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }
    private System.Collections.IEnumerator MovePlayer(Vector2 direction)
    {
        SaveState();
        isMoving = true;
        targetPosition = rb.position + direction * moveDistance;
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        if (IsPathClear(targetPosition) && (hitCollider == null || hitCollider.CompareTag("Goal")))
        {
            rb.MovePosition(targetPosition);
            stepCounter?.IncrementStepCounter();
            if (direction == Vector2.left)
            {
                spriteRenderer.flipX = true;
                spriteRenderer.sprite = defaultSprite;
            }
            else if (direction == Vector2.right)
            {
                spriteRenderer.flipX = false; 
                spriteRenderer.sprite = defaultSprite;
            }
            else if (direction == Vector2.up)
            {
                spriteRenderer.sprite = upSprite;
            }
            else if (direction == Vector2.down)
            {
                spriteRenderer.sprite = defaultSprite; 
            }
        }
        else if (hitCollider != null && hitCollider.CompareTag("Box"))
        {
            BoxMovement box = hitCollider.GetComponent<BoxMovement>();
            Vector2 boxTargetPosition = (Vector2)hitCollider.transform.position + direction * moveDistance;
            if (box != null && box.IsPathClear(boxTargetPosition, true))
            {
                box.MoveBox(boxTargetPosition);
                rb.MovePosition(targetPosition);
                stepCounter?.IncrementStepCounter();
                if (direction == Vector2.left)
                {
                    spriteRenderer.flipX = true; 
                    spriteRenderer.sprite = defaultSprite;
                }
                else if (direction == Vector2.right)
                {
                    spriteRenderer.flipX = false; 
                    spriteRenderer.sprite = defaultSprite;
                }
            }
        }
        yield return new WaitForFixedUpdate();
        isMoving = false;
    }
    private void SaveState()
    {
        List<BoxState> boxStates = new List<BoxState>();
        foreach (var box in FindObjectsOfType<BoxMovement>())
        {
            boxStates.Add(new BoxState(box.transform.position));
        }
        history.Push(new GameState(rb.position, boxStates));
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
        }
    }
    private bool IsPathClear(Vector2 targetPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        return hitCollider == null || (!hitCollider.CompareTag("Wall") && !hitCollider.CompareTag("Box"));
    }
}