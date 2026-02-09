using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VerticallyActivatedPlatform2D : MonoBehaviour
{
    [SerializeField] private PressurePlate2D plate;

    [Header("Movement")]
    [SerializeField] private float upOffset = 3f;
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 upPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        upPos = startPos + Vector2.up * upOffset;

        // For moving platforms, kinematic is usually best
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        if (plate == null) return;

        Vector2 target = plate.IsPressed ? upPos : startPos;
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}