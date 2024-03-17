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
    private float waitTime; // Time to wait before moving again.
    private float waitTimer;
    private bool isIdle = true;

    private Transform playerTransform;
    private bool isMovingToHole = false;

    private float hungerReductionAmount = 5f;

    private WinterReserves hungerScript;

    public GameObject sfxPrefab;

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

        hungerScript = FindObjectOfType<WinterReserves>();
        if (hungerScript == null)
        {
            //Debug.LogError("Hunger script not found!");
        }
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
        //Debug.Log("Current State: " + currentState);
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
            currentState = State.Idle;
            // Ensure the next random destination is safe from the player
            SetRandomDestination(true);
        }
    }

    void IdleBehavior()
    {
        if (currentState == State.Idle)
        {
            if (waitTimer <= 0)
            {
                SetRandomDestination(true);
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
        // Calculate movement direction
        Vector3 moveDirection = destination - transform.position;

        // Flip sprite based on move direction along the x-axis
        if (moveDirection.x < 0) // Moving left
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveDirection.x > 0) // Moving right
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        // Move towards the destination
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void SetRandomDestination(bool ensureSafeFromPlayer = false)
    {
        Vector3 potentialDestination;
        do
        {
            potentialDestination = transform.position + new Vector3(Random.Range(-randomDestinationRadius, randomDestinationRadius), Random.Range(-randomDestinationRadius, randomDestinationRadius), 0);
        }
        while (ensureSafeFromPlayer && Vector3.Distance(potentialDestination, playerTransform.position) < minDistanceToPlayer);

        randomDestination = potentialDestination;

        // Assign a new random wait time between 1 and 2 seconds each time a new destination is set.
        waitTime = Random.Range(1f, 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RabbitHole"))
        {
            //Debug.Log("collide with hole");
            Destroy(gameObject); // Destroy the rabbit
        }

        if (other.CompareTag("Player"))
        {
            if (hungerScript != null)
            {
                hungerScript.ReduceHunger(hungerReductionAmount);
                Instantiate(sfxPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject); // Destroy the rabbit object
            }
            else
            {
                Debug.LogError("Hunger script not found on player!");
            }
        }
    }
}