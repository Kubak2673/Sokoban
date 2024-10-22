using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineConfinerWalls : MonoBehaviour
{
    public string wallTag = "Wall";  // Tag used to identify walls
    public float padding = 0.5f;     // Padding to keep some distance between camera and walls

    private CinemachineConfiner2D confiner;
    private PolygonCollider2D confinerCollider;

    void Start()
    {
        confiner = GetComponent<CinemachineConfiner2D>();

        if (confiner == null)
        {
            Debug.LogError("No Cinemachine Confiner2D component found!");
            return;
        }

        confinerCollider = new GameObject("ConfinerCollider").AddComponent<PolygonCollider2D>();
        confinerCollider.gameObject.transform.SetParent(transform);
        confinerCollider.isTrigger = true;

        confiner.m_BoundingShape2D = confinerCollider;

        UpdateConfinerBounds();
    }

    void UpdateConfinerBounds()
    {
        // Find all objects tagged as "Wall"
        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);
        
        if (walls.Length == 0)
        {
            Debug.LogWarning("No walls found with the tag: " + wallTag);
            return;
        }

        // Calculate the boundaries based on the position of walls
        Bounds bounds = new Bounds(walls[0].transform.position, Vector3.zero);

        foreach (GameObject wall in walls)
        {
            bounds.Encapsulate(wall.GetComponent<Renderer>().bounds);
        }

        // Adjust bounds with padding
        bounds.Expand(padding);

        // Create a polygonal boundary based on the bounds
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(bounds.min.x, bounds.min.y); // Bottom-left
        points[1] = new Vector2(bounds.min.x, bounds.max.y); // Top-left
        points[2] = new Vector2(bounds.max.x, bounds.max.y); // Top-right
        points[3] = new Vector2(bounds.max.x, bounds.min.y); // Bottom-right

        confinerCollider.SetPath(0, points);

        // The path cache is automatically updated, so we don't need InvalidatePathCache.
    }
}
