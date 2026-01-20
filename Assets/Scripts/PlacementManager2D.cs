using UnityEngine;
using UnityEngine.InputSystem;


public class PlacementManager2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject platformPrefab;

    [Header("Placement Settings")]
    [SerializeField] private float rotateStepDegrees = 90f;
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float ghostAlpha = 0.5f;
    [SerializeField] private float ghostRotateSpeedDegPerSec = 540f; // smooth speed
    private float targetRotationZ;
    private bool isPlacing;
    private GameObject ghost;
    private float currentRotationZ;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (!isPlacing || ghost == null) return;

        // Follow mouse
        Vector2 mouseScreen2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreen = new Vector3(mouseScreen2D.x, mouseScreen2D.y, -cam.transform.position.z);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        if (snapToGrid)
        {
            mouseWorld.x = Mathf.Round(mouseWorld.x / gridSize) * gridSize;
            mouseWorld.y = Mathf.Round(mouseWorld.y / gridSize) * gridSize;
        }

        ghost.transform.position = mouseWorld;
        ghost.transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);

        // Rotate with scroll wheel (set target)
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) targetRotationZ += rotateStepDegrees;      // CCW
        else if (scroll < 0f) targetRotationZ -= rotateStepDegrees; // CW

        // Smoothly rotate ghost toward target
        currentRotationZ = Mathf.MoveTowardsAngle(
            currentRotationZ,
            targetRotationZ,
            ghostRotateSpeedDegPerSec * Time.deltaTime
        );

        ghost.transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);


        // Confirm placement
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceRealPlatform(mouseWorld, currentRotationZ);
        }

        // Cancel placement
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            StopPlacing();
        }
    }

    public void StartPlacing()
    {
        if (platformPrefab == null) return;

        isPlacing = true;
        currentRotationZ = 0f;
        targetRotationZ = 0f;
        currentRotationZ = 0f;

        if (ghost != null) Destroy(ghost);

        ghost = Instantiate(platformPrefab);
        ghost.name = "GhostPlatform";

        // Make ghost not collide
        var col = ghost.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        var rb = ghost.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        // Make ghost transparent
        var sr = ghost.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = ghostAlpha;
            sr.color = c;
        }
    }

    private void PlaceRealPlatform(Vector3 pos, float rotZ)
    {
        GameObject placed = Instantiate(platformPrefab, pos, Quaternion.Euler(0f, 0f, rotZ));
        placed.name = "Platform";

        var col = placed.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        var rb = placed.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;  // gravity works now
            rb.gravityScale = 1f;                  // adjust if you want heavier/lighter
            // rb.freezeRotation = true;            // optional: keep it from tipping
        }
    }


    public void StopPlacing()
    {
        isPlacing = false;
        if (ghost != null) Destroy(ghost);
        ghost = null;
    }
}
