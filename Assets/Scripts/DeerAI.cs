using System.Collections;
using UnityEngine;

public class DeerAI : MonoBehaviour
{
    public enum State { Idle, Alert, Fleeing }
    public State currentState = State.Idle;

    public float alertRadius = 3f;
    public float fleeTime = 3f;
    public float minDistanceBeforeFleeing = 1.5f;
    public Sprite idleSprite;
    public Sprite alertSprite;
    public Sprite fleeingSprite;

    public float fleeSpeed = 4f;
    public float timeToIncreaseSpeed = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    private Rigidbody2D playerRb;
    private Vector2 lastPlayerPosition;
    private float playerLastMovedTime;
    private float playerLastStationaryTime;
    private bool playerIsMoving;
    private float playerContinuousMoveStartTime; // Tracks when the player starts moving continuously within the alert radius
    private bool playerStartedMoving = false; // Indicates if the player started moving within the alert radius
    private bool playerHasBeenMovingFor1Sec = false;
    private float movementStartTime;

    private float lastDirectionChangeTime = 0f;
    private Vector2 fleeDirection;
    private bool changeDirection;

    private Hunger hungerScript;
    private float hungerReductionAmount = 50f;

    private float nextMoveTime = 0f; // When the next move should happen
    private float moveInterval = 0f; // The interval until the next move
    private Vector2 moveDirection = Vector2.zero; // The direction of the move

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerLastMovedTime = Time.time;
        playerLastStationaryTime = Time.time;

        hungerScript = FindObjectOfType<Hunger>();
        if (hungerScript == null)
        {
            Debug.LogError("Hunger script not found!");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= 1.8f && distanceToPlayer < 2.5f && playerRb.velocity == Vector2.zero)
        {
            minDistanceBeforeFleeing = 1f;
        }

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(distanceToPlayer);
                HandleIdleMovement();
                break;
            case State.Alert:
                HandleAlertState(distanceToPlayer);
                break;
            case State.Fleeing:
                HandleFleeingState(distanceToPlayer);
                break;
        }

        spriteRenderer.sprite = currentState switch
        {
            State.Idle => idleSprite,
            State.Alert => alertSprite,
            State.Fleeing => fleeingSprite,
            _ => spriteRenderer.sprite
        };

        // Update player movement tracking
        CheckPlayerMovement();
    }

    private void CheckPlayerMovement()
    {
        if (Vector2.Distance(player.transform.position, rb.position) <= alertRadius)
        {
            if (player.transform.hasChanged)
            {
                player.transform.hasChanged = false;
                playerLastMovedTime = Time.time;
                playerIsMoving = true;
            }
            else if (playerIsMoving && (Time.time - playerLastMovedTime >= 0.5f))
            {
                playerIsMoving = false;
                playerLastStationaryTime = Time.time;
            }
        }
    }

    private void HandleIdleState(float distanceToPlayer)
    {
        if (distanceToPlayer <= alertRadius && playerIsMoving)
        {
            currentState = State.Alert;
        }
    }

    private void HandleAlertState(float distanceToPlayer)
    {
        if (distanceToPlayer > alertRadius)
        {
            currentState = State.Idle;
        }
        else if (distanceToPlayer <= minDistanceBeforeFleeing || (playerIsMoving && Time.time - playerLastMovedTime > 1f))
        {
            currentState = State.Fleeing;
        }
        else if (!playerIsMoving && Time.time - playerLastStationaryTime >= 0.5f)
        {
            currentState = State.Idle;
        }
    }

    private void HandleFleeingState(float distanceToPlayer)
    {
        if (distanceToPlayer > alertRadius)
        {
            currentState = State.Idle;
        }
        else
        {
            StartCoroutine(Flee());
        }
    }
    IEnumerator Flee()
    {
        fleeDirection = (transform.position - player.transform.position).normalized;
        float fleeStartTime = Time.time;
        lastDirectionChangeTime = Time.time;

        while (Time.time - fleeStartTime < fleeTime)
        {
            if (Time.time - lastDirectionChangeTime >= 0.5f)
            {
                changeDirection = !changeDirection; // Toggle the direction change for next interval
                fleeDirection = Quaternion.Euler(0, 0, changeDirection ? 60 : -60) * fleeDirection;
                lastDirectionChangeTime = Time.time;
            }

            float elapsedTime = Time.time - fleeStartTime;
            float currentSpeed = Mathf.Lerp(0, fleeSpeed, Mathf.Min(1, elapsedTime / timeToIncreaseSpeed));
            rb.velocity = fleeDirection * currentSpeed;

            // Determine the direction of the flee to flip the sprite accordingly
            spriteRenderer.flipX = fleeDirection.x < 0;

            yield return null;
        }

        rb.velocity = Vector2.zero;
        currentState = State.Idle; // Return to idle after fleeing
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hungerScript != null)
        {
            hungerScript.ReduceHunger(hungerReductionAmount);
            Destroy(gameObject); // Destroy the deer object
        }
    }
    private void HandleIdleMovement()
    {
        if (Time.time >= nextMoveTime)
        {
            // Calculate the next move interval
            moveInterval = Random.Range(0f, 2f); // Random time between 0 and 2 seconds
            nextMoveTime = Time.time + moveInterval;

            // Choose a random direction
            float angle = Random.Range(0f, 360f);
            moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Apply the movement
            StartCoroutine(MoveDeer(moveDirection, 0.1f));
        }
    }

    IEnumerator MoveDeer(Vector2 direction, float distance)
    {
        Vector2 startPosition = rb.position;
        Vector2 endPosition = startPosition + direction.normalized * distance;

        // Move over 0.1 seconds for smoothness, adjust if needed
        float moveDuration = 0.1f;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            rb.position = Vector2.Lerp(startPosition, endPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}