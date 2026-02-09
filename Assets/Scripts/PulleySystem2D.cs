using UnityEngine;

public class PulleySystem2D : MonoBehaviour
{
    [Header("Platforms")]
    [SerializeField] private Transform platformA;
    [SerializeField] private Transform platformB;

    [Header("Sensors (triggers on top)")]
    [SerializeField] private PulleyWeightSensor2D sensorA;
    [SerializeField] private PulleyWeightSensor2D sensorB;

    [Header("Motion")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float deadzoneMassDifference = 0.05f;

    [Header("Limits")]
    [SerializeField] private float platformAMinY;
    [SerializeField] private float platformAMaxY;
    [SerializeField] private float platformBMinY;
    [SerializeField] private float platformBMaxY;

    [Header("Optional feel")]
    [SerializeField] private float maxMassDifferenceForFullSpeed = 5f;

    private void Reset()
    {
        speed = 2.5f;
        deadzoneMassDifference = 0.05f;
        maxMassDifferenceForFullSpeed = 5f;
    }

    private void FixedUpdate()
    {
        if (platformA == null || platformB == null || sensorA == null || sensorB == null) return;

        float massA = sensorA.TotalMass;
        float massB = sensorB.TotalMass;

        float diff = massA - massB; // positive means A is heavier
        if (Mathf.Abs(diff) < deadzoneMassDifference) return;

        float t = Mathf.Clamp(diff / Mathf.Max(0.001f, maxMassDifferenceForFullSpeed), -1f, 1f);

        float delta = t * speed * Time.fixedDeltaTime;

        // If A is heavier, A should move down, B should move up
        MovePlatformPair(-delta, +delta);
    }

    private void MovePlatformPair(float deltaAY, float deltaBY)
    {
        Vector3 a = platformA.position;
        Vector3 b = platformB.position;

        float targetAY = a.y + deltaAY;
        float targetBY = b.y + deltaBY;

        // Clamp to limits
        float clampedAY = Mathf.Clamp(targetAY, platformAMinY, platformAMaxY);
        float clampedBY = Mathf.Clamp(targetBY, platformBMinY, platformBMaxY);

        // Rope constraint feel
        // If one side hits a limit, the other side should also stop for that step.
        // We do that by applying only the “allowed” movement.
        float appliedAY = clampedAY - a.y;
        float appliedBY = clampedBY - b.y;

        float applied = 0f;

        // We want equal magnitude and opposite direction.
        // Pick the smaller magnitude movement that both sides can do this frame.
        float magA = Mathf.Abs(appliedAY);
        float magB = Mathf.Abs(appliedBY);
        float mag = Mathf.Min(magA, magB);

        if (mag <= 0f) return;

        applied = Mathf.Sign(appliedAY) * mag; // sign of A determines direction

        a.y += applied;
        b.y -= applied;

        platformA.position = a;
        platformB.position = b;
    }
}
