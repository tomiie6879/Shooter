using UnityEngine;

public sealed class SpiritOrbit : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField, Min(0f)] private float radius = 0.35f;
    [SerializeField] private float angularSpeed = 90f;
    [SerializeField] private float startingAngle = 0f;

    private float angle;
    private float localZ;

    private void Awake()
    {
        angle = startingAngle;
        localZ = transform.localPosition.z;
        UpdatePosition();
    }

    private void Update()
    {
        angle = Mathf.Repeat(
            angle + angularSpeed * Time.deltaTime,
            360f
        );

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float radians = angle * Mathf.Deg2Rad;

        transform.localPosition = new Vector3(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius,
            localZ
        );
    }
}