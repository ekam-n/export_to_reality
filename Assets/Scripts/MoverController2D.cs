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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        // Optional: visualize it in Scene view
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red);

        return hit.collider != null;
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