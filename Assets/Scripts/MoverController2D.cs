using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MoverController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpImpulse = 10f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;

        // Start just inside the bottom of the collider
        Vector2 origin = new Vector2(b.center.x, b.min.y + 0.02f);

        // A thin box as wide as the player (slightly smaller so it doesn't catch edges)
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.02f);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null && hit.collider != col;
    }

    // Called by PlayerInput (Send Messages) when the "Move" action changes
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Called by PlayerInput (Send Messages) when the "Jump" action is performed
    public void OnJump(InputValue value)
    {
        if (value.isPressed) jumpQueued = true;
    }

    private void FixedUpdate()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (jumpQueued)
        {
            jumpQueued = false;

            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            }
        }
    }
}