using UnityEngine;

public class PlayerDynamicSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float checkRadius = 1f; // Radius to check for nearby obstacles
    public LayerMask obstacleLayer;
    public float yOffset = 0.5f; // Adjust in Inspector based on your needs

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Note: No need to set obstacleLayer here if you're assigning it in the Inspector
    }

    void Update()
    {
        AdjustSortingOrderBasedOnClosestObstacle();
    }

    void AdjustSortingOrderBasedOnClosestObstacle()
    {
        // Find all obstacles within checkRadius
        Collider2D[] obstaclesInRange = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, yOffset, 0), checkRadius, obstacleLayer);

        if (obstaclesInRange.Length > 0)
        {
            // Use the player's adjusted position for sorting calculations
            Vector3 adjustedPosition = transform.position + new Vector3(0, yOffset, 0);

            // Initially, assume the player's sorting order is based on its Y position (this might be adjusted to fit your specific needs)
            int sortingOrderBasedOnY = Mathf.RoundToInt(adjustedPosition.y * -100);

            // Assign a dynamic sorting order based on the calculated value
            spriteRenderer.sortingOrder = sortingOrderBasedOnY;
        }
    }
}