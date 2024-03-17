using UnityEngine;

public class PlayerDynamicSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float checkRadius = 1f; // Radius to check for nearby obstacles
    public LayerMask obstacleLayer;

    private float yOffset;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
        AdjustSortingOrderBasedOnClosestObstacle();
    }

    void AdjustSortingOrderBasedOnClosestObstacle()
    {
        // Find all obstacles within checkRadius
        Collider2D[] obstaclesInRange = Physics2D.OverlapCircleAll(transform.position, checkRadius, obstacleLayer);

        // Keep track of the closest obstacle and its distance
        Collider2D closestObstacle = null;
        float closestDistance = float.MaxValue;

        foreach (var obstacle in obstaclesInRange)
        {
            float distance = Vector2.Distance(transform.position, obstacle.transform.position);
            if (distance < closestDistance)
            {
                closestObstacle = obstacle;
                closestDistance = distance;
            }
        }

        if (closestObstacle != null)
        {
            // Determine sorting order based on Y position relative to the closest obstacle
            if (transform.position.y < closestObstacle.transform.position.y - yOffset)
            {
                // Object is behind the tree
                spriteRenderer.sortingOrder = closestObstacle.GetComponent<SpriteRenderer>().sortingOrder +1;
            }
            else
            {
                // Object is in front of the tree
                spriteRenderer.sortingOrder = closestObstacle.GetComponent<SpriteRenderer>().sortingOrder -1;
            }
        }
    }
}