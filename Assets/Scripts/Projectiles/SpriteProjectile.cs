using UnityEngine;

// Reusable for fireballs, arrows and other sprite-based projectiles.
[RequireComponent(typeof(SpriteRenderer))]
public sealed class SpriteProjectile : PooledProjectile
{
    [Tooltip("Enable when the artwork should turn towards the target.")]
    [SerializeField] private bool faceDirection = true;
    [Tooltip("0 if artwork points right; -90 if artwork points up.")]
    [SerializeField] private float angleOffset;
    private Animator animator;

    protected override void OnLaunched()
    {
        transform.rotation = faceDirection
            ? Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset)
            : Quaternion.identity;
        if (animator == null) animator = GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.Rebind();
            animator.Update(0);
        }
    }
}
