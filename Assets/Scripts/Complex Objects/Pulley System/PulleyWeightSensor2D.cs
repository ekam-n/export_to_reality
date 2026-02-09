using System.Collections.Generic;
using UnityEngine;

public class PulleyWeightSensor2D : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private readonly HashSet<Rigidbody2D> bodies = new HashSet<Rigidbody2D>();

    public float TotalMass
    {
        get
        {
            float sum = 0f;
            foreach (var rb in bodies)
            {
                if (rb != null) sum += rb.mass;
            }
            return sum;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;

        if (debugLogs)
        {
            Debug.Log(
                $"[PulleySensor ENTER] sensor={name} other={other.name} " +
                $"isTrigger(other)={other.isTrigger} rb={(rb ? rb.name : "NULL")} " +
                $"rbType={(rb ? rb.bodyType.ToString() : "NA")} mass={(rb ? rb.mass.ToString("0.###") : "NA")}"
            );
        }

        if (rb == null) return;

        bool added = bodies.Add(rb);

        if (debugLogs)
        {
            Debug.Log(
                $"[PulleySensor TRACK] sensor={name} added={added} trackedCount={bodies.Count} totalMass={TotalMass:0.###}"
            );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;

        if (debugLogs)
        {
            Debug.Log(
                $"[PulleySensor EXIT] sensor={name} other={other.name} rb={(rb ? rb.name : "NULL")}"
            );
        }

        if (rb == null) return;

        bool removed = bodies.Remove(rb);

        if (debugLogs)
        {
            Debug.Log(
                $"[PulleySensor TRACK] sensor={name} removed={removed} trackedCount={bodies.Count} totalMass={TotalMass:0.###}"
            );
        }
    }

    // Optional: helps catch cases where Enter never fires due to spawn-overlap weirdness
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!debugLogs) return;

        // Uncomment if you want spammy continuous proof:
        // var rb = other.attachedRigidbody;
        // Debug.Log($"[PulleySensor STAY] sensor={name} other={other.name} rb={(rb ? rb.name : "NULL")} totalMass={TotalMass:0.###}");
    }
}
