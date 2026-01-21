using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementManager2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Placeable Prefabs (buttons select an index)")]
    [SerializeField] private GameObject[] platformPrefabs;

    [Header("Limits")]
    [SerializeField] private int maxPlatformsInScene = 5;

    [Header("Placement Settings")]
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float rotateStepDegrees = 90f;
    [SerializeField] private float ghostRotateSpeedDegPerSec = 540f;
    [SerializeField] private float ghostAlpha = 0.5f;

    [Header("Removal Settings")]
    [Tooltip("What layers count as removable platforms. Put your placed platforms on this layer.")]
    [SerializeField] private LayerMask removableMask;
    [SerializeField] private float clickRadius = 0.1f;

    private readonly List<GameObject> placedPlatforms = new();

    private bool isPlacing;
    private int selectedIndex = 0;

    private GameObject ghost;
    private float currentRotationZ;
    private float targetRotationZ;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // If we're placing, update ghost + placement controls
        if (isPlacing && ghost != null)
        {
            UpdateGhostFollowAndRotation();

            // Ignore placing clicks if pointer is over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Left click place (if under limit)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (placedPlatforms.Count >= maxPlatformsInScene)
                {
                    // Option A: Do nothing when max reached
                    // Option B: delete oldest then place
                    // RemovePlatform(placedPlatforms[0]);
                    // (uncomment above 2 lines if you prefer "replace oldest")
                    return;
                }

                Vector3 pos = ghost.transform.position;
                PlaceRealPlatform(pos, targetRotationZ);
            }

            // Right click cancels placement mode
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                StopPlacing();
            }
        }
        else
        {
            // Not placing: allow removal by click (optional)
            // Ignore if clicking UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Right click delete a platform under mouse
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                TryRemovePlatformUnderMouse();
            }
        }
    }

    // UI Buttons call this with a different index
    public void StartPlacing(int prefabIndex)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0) return;
        if (prefabIndex < 0 || prefabIndex >= platformPrefabs.Length) return;

        selectedIndex = prefabIndex;
        isPlacing = true;

        currentRotationZ = 0f;
        targetRotationZ = 0f;

        if (ghost != null) Destroy(ghost);

        // Create ghost
        ghost = Instantiate(platformPrefabs[selectedIndex]);
        ghost.name = "GhostPlatform";

        // Disable collisions / physics on ghost
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

    public void StopPlacing()
    {
        isPlacing = false;
        if (ghost != null) Destroy(ghost);
        ghost = null;
    }

    private void UpdateGhostFollowAndRotation()
    {
        Vector2 mouseScreen2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreen = new(mouseScreen2D.x, mouseScreen2D.y, -cam.transform.position.z);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        if (snapToGrid)
        {
            mouseWorld.x = Mathf.Round(mouseWorld.x / gridSize) * gridSize;
            mouseWorld.y = Mathf.Round(mouseWorld.y / gridSize) * gridSize;
        }

        ghost.transform.position = mouseWorld;

        // Scroll sets target rotation in steps
        float scrollY = Mouse.current.scroll.ReadValue().y;
        if (scrollY > 0f) targetRotationZ += rotateStepDegrees;      // CCW
        else if (scrollY < 0f) targetRotationZ -= rotateStepDegrees; // CW

        // Smooth rotate toward target
        currentRotationZ = Mathf.MoveTowardsAngle(
            currentRotationZ,
            targetRotationZ,
            ghostRotateSpeedDegPerSec * Time.deltaTime
        );

        ghost.transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
    }

    private void PlaceRealPlatform(Vector3 pos, float rotZ)
    {
        GameObject prefab = platformPrefabs[selectedIndex];
        GameObject placed = Instantiate(prefab, pos, Quaternion.Euler(0f, 0f, rotZ));
        placed.name = $"Platform_{selectedIndex}";

        // Ensure it is removable (must have PlaceablePlatform)
        if (placed.GetComponent<PlaceablePlatform>() == null)
        {
            placed.AddComponent<PlaceablePlatform>();
        }

        // Make sure physics is active (dynamic)
        var rb = placed.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
        }

        var col = placed.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        placedPlatforms.Add(placed);
    }

    private void TryRemovePlatformUnderMouse()
    {
        Vector2 mouseScreen2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreen = new(mouseScreen2D.x, mouseScreen2D.y, -cam.transform.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        // Overlap test (small radius). Requires removable platforms to be on removableMask layers.
        Collider2D hit = Physics2D.OverlapCircle(mouseWorld, clickRadius, removableMask);
        if (hit == null) return;

        // Only remove if it’s one of our placed platforms
        PlaceablePlatform marker = hit.GetComponentInParent<PlaceablePlatform>();
        if (marker == null) return;

        RemovePlatform(marker.gameObject);
    }

    private void RemovePlatform(GameObject platform)
    {
        if (platform == null) return;
        placedPlatforms.Remove(platform);
        Destroy(platform);
    }

    // Optional helper for a "Remove Mode" UI button
    public void RemoveMode()
    {
        StopPlacing();
        // Now right-click will delete under mouse
    }

    // Optional: clear all placed platforms
    public void ClearAllPlaced()
    {
        for (int i = placedPlatforms.Count - 1; i >= 0; i--)
        {
            if (placedPlatforms[i] != null) Destroy(placedPlatforms[i]);
        }
        placedPlatforms.Clear();
    }

    // Optional: visualize removal radius in Scene view
    void OnDrawGizmosSelected()
    {
        if (cam == null) return;
        Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
    }
}
