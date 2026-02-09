using UnityEngine;

public class FixedZoneCameraController2D : MonoBehaviour
{
    [SerializeField] private Transform[] zoneAnchors;
    [SerializeField] private int currentZoneIndex = 0;

    [Header("Camera Ref")]
    [SerializeField] private Camera cam;

    [Header("Smooth Move")]
    [SerializeField] private float moveSmoothTime = 0.25f;
    [SerializeField] private float z = -10f;

    [Header("Smooth Zoom")]
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float zoomSmoothTime = 0.25f;

    private Vector3 moveVel;
    private float zoomVel;

    private void Reset()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (zoneAnchors == null || zoneAnchors.Length == 0) return;
        if (cam == null) return;

        currentZoneIndex = Mathf.Clamp(currentZoneIndex, 0, zoneAnchors.Length - 1);

        // Move
        Vector3 targetPos = zoneAnchors[currentZoneIndex].position;
        targetPos.z = z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVel, moveSmoothTime);

        // Zoom
        if (enableZoom)
        {
            var anchor = zoneAnchors[currentZoneIndex].GetComponent<CameraZone2D>();
            if (anchor != null)
            {
                cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, anchor.orthoSize, ref zoomVel, zoomSmoothTime);
            }
        }
    }

    public void SetZone(int index)
    {
        currentZoneIndex = Mathf.Clamp(index, 0, zoneAnchors.Length - 1);
    }
}
