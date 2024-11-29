using UnityEngine;
public class BoxMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask obstacleLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void MoveBox(Vector2 targetPosition)
    {
        rb.MovePosition(targetPosition);
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
}