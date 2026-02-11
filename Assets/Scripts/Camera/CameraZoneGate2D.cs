using System.Collections.Generic;
using UnityEngine;

public class CameraZoneGate2D : MonoBehaviour
{
    [SerializeField] private FixedZoneCameraController2D cameraController;
    [SerializeField] private int leftZoneIndex = 0;
    [SerializeField] private int rightZoneIndex = 1;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform respawnPoint;

    // Track entry side per collider (supports multiple players if needed)
    private readonly Dictionary<Collider2D, bool> enteredFromLeft = new Dictionary<Collider2D, bool>();

    private void Reset()
    {
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null) bc.isTrigger = true;
    }

    private void Awake()
    {
        if (cameraController == null)
            cameraController = FindFirstObjectByType<FixedZoneCameraController2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        float gateX = transform.position.x;
        float x = other.bounds.center.x;

        // true if the player started on the left side when they entered the trigger
        enteredFromLeft[other] = (x < gateX);

        var mover = other.GetComponent<MoverController2D>();
        if (mover != null)
        {
            mover.SetRespawnFromGate(respawnPoint != null ? respawnPoint : transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cameraController == null) return;

        float gateX = transform.position.x;
        float x = other.bounds.center.x;

        bool wasFromLeft = false;
        if (enteredFromLeft.TryGetValue(other, out bool val))
            wasFromLeft = val;

        // If they entered from left and exited on right -> moved left->right
        if (wasFromLeft && x > gateX)
            cameraController.SetZone(rightZoneIndex);

        // If they entered from right and exited on left -> moved right->left
        else if (!wasFromLeft && x < gateX)
            cameraController.SetZone(leftZoneIndex);

        enteredFromLeft.Remove(other);
    }
}
