using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask obstacleLayer;

    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite onGoalSprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MoveBox(Vector2 targetPosition)
    {
        rb.MovePosition(targetPosition);
        UpdateSprite(targetPosition);
    }

    public bool IsPathClear(Vector2 targetPosition, bool checkGoals = false)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        if (checkGoals && hitCollider != null && hitCollider.CompareTag("Goal"))
        {
            return true;
        }
        return hitCollider == null || (!hitCollider.CompareTag("Wall") && !hitCollider.CompareTag("Box"));
    }

    private void UpdateSprite(Vector2 targetPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, obstacleLayer);
        if (hitCollider != null && hitCollider.CompareTag("Goal"))
        {
            spriteRenderer.sprite = onGoalSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
