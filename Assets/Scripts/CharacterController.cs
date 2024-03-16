using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Movement speed of the character.
    private Rigidbody2D rb;
    private Vector2 movement = new Vector2();
    private bool facingRight = true; // Track which direction the player is facing.
    
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component attached to the character.
    }

    private void Update()
    {
        movement = new Vector2();

        // Check for ZQSD or arrow key input.
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movement.y = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement.y = -1;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1;
            if (facingRight) // If moving left and facing right, flip the sprite.
            {
                Flip();
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1;
            if (!facingRight) // If moving right and not facing right, flip the sprite.
            {
                Flip();
            }
        }

        // Normalize the movement vector to ensure consistent movement speed in all directions.
        movement.Normalize();

        // Update the Animator based on the movement.
        animator.SetBool("IsRunningLeftOrRight", movement.magnitude > 0);
        
        rb.velocity = movement * moveSpeed;
    }

    private void Flip()
    {
        facingRight = !facingRight; // Toggle the state.
        Vector3 theScale = transform.localScale; // Get the local scale.
        theScale.x *= -1; // Flip the x component.
        transform.localScale = theScale; // Apply the flipped scale.
    }
}