using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
    public float timeToIncreaseSpeed = 1f; // Time taken to reach full speed

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    private Vector2 lastPlayerPosition;
    private float playerLastMovedTime;

    private Hunger hungerScript;
    private float hungerReductionAmount = 50f;

    private bool changeDirection = true; // Flag to alternate direction change

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player"); 
        lastPlayerPosition = player.transform.position;

        hungerScript = FindObjectOfType<Hunger>();
        if (hungerScript == null)
        {
            Debug.LogError("Hunger script not found!");
        }
    }

    void Update()
    {
        // Check if player has moved
        if ((Vector2)player.transform.position != lastPlayerPosition)
        {
            playerLastMovedTime = Time.time;
            lastPlayerPosition = player.transform.position;
        }

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Alert:
                HandleAlertState();
                break;
            case State.Fleeing:
                HandleFleeingState();
                break;
        }

        // Flip sprite based on deer movement direction
        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true; // Flip sprite to face left
        }
        else if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false; // Normal sprite orientation (face right)
        }
    }

    void HandleIdleState()
    {
        spriteRenderer.sprite = idleSprite;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= alertRadius && Time.time - playerLastMovedTime <= 0.5f)
        {
            currentState = State.Alert;
        }
    }

    void HandleAlertState()
    {
        spriteRenderer.sprite = alertSprite;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance > alertRadius || Time.time - playerLastMovedTime > 0.5f)
        {
            currentState = State.Idle;
        }
        else if (Time.time - playerLastMovedTime <= 0.5f && distance <= minDistanceBeforeFleeing)
        {
            currentState = State.Fleeing;
            StartCoroutine(Flee());
        }
    }

    void HandleFleeingState()
    {
        spriteRenderer.sprite = fleeingSprite;
        // Movement is handled in the Flee coroutine
    }

    IEnumerator Flee()
    {
        Vector2 fleeDirection = (transform.position - player.transform.position).normalized;
        float fleeStartTime = Time.time;
        float lastDirectionChangeTime = Time.time;

        // Gradually increase speed to full speed in the first 2 seconds
        while (Time.time - fleeStartTime < fleeTime)
        {
            if (Time.time - lastDirectionChangeTime >= 0.5f)
            {
                changeDirection = !changeDirection; // Toggle the direction change for next interval
                fleeDirection = Quaternion.Euler(0, 0, changeDirection ? 60 : -60) * fleeDirection;
                lastDirectionChangeTime = Time.time;
            }

            float currentSpeed = Mathf.Lerp(0, fleeSpeed, (Time.time - fleeStartTime) / timeToIncreaseSpeed);
            rb.velocity = fleeDirection.normalized * currentSpeed;

            // Ensure the speed does not exceed fleeSpeed after the initial speed increase period
            if (Time.time - fleeStartTime >= timeToIncreaseSpeed)
            {
                currentSpeed = fleeSpeed;
            }

            yield return null;
        }

        rb.velocity = Vector2.zero;
        currentState = State.Idle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            if (hungerScript != null)
            {
                hungerScript.ReduceHunger(hungerReductionAmount);
                Destroy(gameObject); // Destroy the deer object
            }
            else
            {
                Debug.LogError("Hunger script not found on player!");
            }
        }
    }
}