using UnityEngine;

// Lightning appearance only. Flight, hits and pool lifetime are shared.
[RequireComponent(typeof(LineRenderer))]
public sealed class LightningProjectile : PooledProjectile
{
    [SerializeField, Min(.01f)] private float boltLength = .5f;
    [SerializeField, Min(0)] private float zigzag = .07f;
    [SerializeField, Min(.01f)] private float width = 2;
    private LineRenderer line;
    protected override void OnLaunched()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        line.widthMultiplier = width;
        line.useWorldSpace = true;
        line.positionCount = 7;
        line.enabled = true;
    }
    protected override void DrawVisual()
    {
        // transform.position là đầu gây sát thương.
        Vector3 head = transform.position;
        Vector3 side = new Vector3(-direction.y, direction.x, 0f);

        // Hiện sẵn một đoạn ngắn từ điểm phát ngay khi sinh.
        float initialLength = Mathf.Min(0.15f, boltLength);
        float length = Mathf.Min(
            boltLength,
            initialLength + traveledDistance
        );

        // Phần đầu hiển thị nhô ra nhẹ phía trước đầu va chạm.
        Vector3 visualHead =
            head + (Vector3)direction * initialLength;

        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);

            Vector3 point =
                visualHead - (Vector3)direction * length * t;

            if (i > 0 && i < line.positionCount - 1)
            {
                point += side * Random.Range(-zigzag, zigzag)
                       * Mathf.Clamp01(length / boltLength);
            }

            line.SetPosition(i, point);
        }
    }
    private void OnValidate()
    {
        LineRenderer renderer = GetComponent<LineRenderer>();

        if (renderer != null)
            renderer.widthMultiplier = width;
    }
    protected override void ResetVisual()
    {
        if (line != null) line.enabled = false;
    }
}
