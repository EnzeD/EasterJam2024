using UnityEngine;

public class PlayerDynamicSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float checkRadius = 5f; // Radius to check for nearby trees
    public LayerMask treeLayer; // Assign in inspector, set to the layer your trees are on

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        AdjustSortingOrderBasedOnClosestTree();
    }

    void AdjustSortingOrderBasedOnClosestTree()
    {
        // Find all trees within checkRadius
        Collider2D[] treesInRange = Physics2D.OverlapCircleAll(transform.position, checkRadius, treeLayer);

        // Keep track of the closest tree and its distance
        Collider2D closestTree = null;
        float closestDistance = float.MaxValue;

        foreach (var tree in treesInRange)
        {
            float distance = Vector2.Distance(transform.position, tree.transform.position);
            if (distance < closestDistance)
            {
                closestTree = tree;
                closestDistance = distance;
            }
        }

        if (closestTree != null)
        {
            // Determine sorting order based on Y position relative to the closest tree
            // You might want to adjust this logic based on your game's needs, e.g., adding an offset
            if (transform.position.y < closestTree.transform.position.y)
            {
                // Player is behind the tree
                spriteRenderer.sortingOrder = closestTree.GetComponent<SpriteRenderer>().sortingOrder +1;
            }
            else
            {
                // Player is in front of the tree
                spriteRenderer.sortingOrder = closestTree.GetComponent<SpriteRenderer>().sortingOrder -1;
            }
        }
    }
}