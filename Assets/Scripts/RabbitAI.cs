using UnityEngine;

public class RabbitAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minDistanceToPlayer = 5f; // Distance at which rabbit perceives danger from the player.
    public float detectionRadius = 5f; // Radius to detect rabbit hole.
    public float randomDestinationRadius = 0.5f;

    private Vector3 randomDestination;
    private Vector3 holeDestination;
    private float waitTime = 2f; // Time to wait before moving again.
    private float waitTimer;
    private bool isIdle = true;

    private Transform playerTransform;
    private bool isMovingToHole = false;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDestination();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isMovingToHole)
        {
            MoveTowards(holeDestination);
        }
        else
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("RabbitHole"))
                {
                    holeDestination = hit.transform.position;
                    isMovingToHole = true;
                    return; // Exit early if a rabbit hole is found
                }
                else
                {
                    isMovingToHole = false;
                }
            }

            //Debug.Log("Distance to player: " + distanceToPlayer);
            if (distanceToPlayer < minDistanceToPlayer)
            {
                // Danger state
                isIdle = false;
                RunFromPlayer();
            }
            else
            {
                // Idle state
                isIdle = true;
                IdleBehavior();
            }
        }        
    }

    void IdleBehavior()
    {
        if (waitTimer <= 0)
        {

            SetRandomDestination();
            waitTimer = waitTime; // Reset wait timer
        }
        else
        {
            waitTimer -= Time.deltaTime; // Decrease timer
            MoveTowards(randomDestination);
        }
    }

    void RunFromPlayer()
    {
        Vector3 directionFromPlayer = transform.position - playerTransform.position;
        Vector3 runDestination = transform.position + directionFromPlayer;

        MoveTowards(runDestination);
    }

    void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void SetRandomDestination()
    {
        // Set a random destination within a range around the rabbit
        randomDestination = transform.position + new Vector3(Random.Range(-randomDestinationRadius, randomDestinationRadius), Random.Range(randomDestinationRadius, randomDestinationRadius), 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RabbitHole"))
        {
            Debug.Log("collide with hole");
            Destroy(gameObject); // Destroy the rabbit
        }
    }
}