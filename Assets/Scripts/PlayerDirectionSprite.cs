using UnityEngine;

public class PlayerDirectionSprite : MonoBehaviour
{
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
        spriteRenderer.sprite = downSprite; // Default sprite
    }

    void Update()
    {
        Vector3 direction = transform.position - lastPosition;

        if (direction.x > 0)
        {
            spriteRenderer.sprite = rightSprite;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.sprite = leftSprite;
        }
        else if (direction.y > 0)
        {
            spriteRenderer.sprite = upSprite;
        }
        else if (direction.y < 0)
        {
            spriteRenderer.sprite = downSprite;
        }

        lastPosition = transform.position;
    }
}
