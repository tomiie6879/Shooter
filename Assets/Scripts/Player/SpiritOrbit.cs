using UnityEngine;

// Positions the spirit only; does not rotate its artwork or FirePoint.
[DefaultExecutionOrder(-10)]
public sealed class SpiritOrbit : MonoBehaviour
{
    [SerializeField, Min(0f)] private float radius = .35f;
    [SerializeField] private float angularSpeed = 90f;
    [SerializeField] private float startingAngle;
    private float angle;
    private Vector3 localCenter;
    private Transform sharedCenter;
    private Vector3 centerOffset;
    private bool configured;

    private void Awake()
    {
        if (configured) return;
        angle = startingAngle;
        localCenter = transform.localPosition;
        UpdatePosition();
    }

    public void Configure(Transform center, Vector3 offset, float orbitRadius,
        float speed, float phase)
    {
        sharedCenter = center;
        centerOffset = offset;
        radius = Mathf.Max(0, orbitRadius);
        angularSpeed = speed;
        angle = phase;
        configured = true;
        enabled = true;
        UpdatePosition();
    }

    private void LateUpdate()
    {
        angle = Mathf.Repeat(angle + angularSpeed * Time.deltaTime, 360f);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float radians = angle * Mathf.Deg2Rad;
        var offset = new Vector3(Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius, 0);
        if (configured && sharedCenter != null)
            transform.position = sharedCenter.TransformPoint(centerOffset) + offset;
        else if (!configured)
            transform.localPosition = localCenter + offset;
    }
}
