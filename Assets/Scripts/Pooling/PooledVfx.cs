using UnityEngine;

[RequireComponent(typeof(PoolItem))]
public sealed class PooledVfx : MonoBehaviour
{
    private ParticleSystem[] particles;
    private TrailRenderer[] trails;
    private Animator[] animators;

    private void CacheComponents()
    {
        if (particles != null)
            return;

        particles = GetComponentsInChildren<ParticleSystem>(true);
        trails = GetComponentsInChildren<TrailRenderer>(true);
        animators = GetComponentsInChildren<Animator>(true);
    }

    public void Play()
    {
        CacheComponents();

        gameObject.SetActive(true);

        foreach (TrailRenderer trail in trails)
            trail.Clear();

        foreach (Animator animator in animators)
        {
            if (!animator.isActiveAndEnabled ||
                animator.runtimeAnimatorController == null)
                continue;

            animator.Rebind();
            animator.Update(0f);
        }

        foreach (ParticleSystem particle in particles)
        {
            if (!particle.gameObject.activeInHierarchy)
                continue;

            particle.Stop(
                false,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particle.Play(false);
        }
    }

    private void OnDisable()
    {
        CacheComponents();

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(
                false,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        foreach (TrailRenderer trail in trails)
            trail.Clear();
    }
}