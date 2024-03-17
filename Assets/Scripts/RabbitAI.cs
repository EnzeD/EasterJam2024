using Unity.VisualScripting;
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

    private enum State
    {
        Idle,
        MovingToHole,
        Danger
    }

    private State currentState = State.Idle;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDestination();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleBehavior();
                break;
            case State.MovingToHole:
                MoveTowards(holeDestination);
                break;
            case State.Danger:
                RunFromPlayer();
                break;
        }

        CheckForHole();
        CheckForDanger();
    }

    void CheckForHole()
    {
        if (currentState != State.MovingToHole)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("RabbitHole"))
                {
                    holeDestination = hit.transform.position;
                    currentState = State.MovingToHole;
                    return; // Exit early if a rabbit hole is found
                }
            }
        }
    }

    void CheckForDanger()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (currentState != State.MovingToHole && distanceToPlayer < minDistanceToPlayer)
        {
            currentState = State.Danger;
        }
        else if (currentState == State.Danger && distanceToPlayer >= minDistanceToPlayer)
        {
            // Only return to Idle if not moving to hole
            currentState = State.Idle;
        }
    }

    void IdleBehavior()
    {
        if (currentState == State.Idle)
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
        randomDestination = transform.position + new Vector3(Random.Range(-randomDestinationRadius, randomDestinationRadius), Random.Range(-randomDestinationRadius, randomDestinationRadius), 0);
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