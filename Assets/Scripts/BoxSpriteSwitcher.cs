using UnityEngine;

public class BoxSpriteSwitcher : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite onGoalSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            spriteRenderer.sprite = onGoalSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
}
