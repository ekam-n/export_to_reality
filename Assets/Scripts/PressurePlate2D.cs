using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate2D : MonoBehaviour
{
    [Tooltip("Which layers are allowed to press the plate (Mover, placed platforms, etc.)")]
    [SerializeField] private LayerMask pressMask;

    public bool IsPressed => _pressing.Count > 0;

    private readonly HashSet<Collider2D> _pressing = new();

    private void Reset()
    {
        // Make sure the plate collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & pressMask) == 0) return;
        _pressing.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _pressing.Remove(other);
    }
}