using UnityEngine;

public class killstreakAdrenaline : killstreakBase
{
    [Header("Adrenaline Rush")]
    [Tooltip("How slow the world becomes. 0.2 = enemies move at 20% speed.")]
    [SerializeField][Range(0.01f, 1f)] private float worldTimeScale = 0.2f;

    private float cachedTimeScale;
    private float cachedFixedDeltaTime;

    protected override void onActivate()
    {
        // Remember original values so we can restore them exactly
        cachedTimeScale = Time.timeScale;
        cachedFixedDeltaTime = Time.fixedDeltaTime;

        // Slow the entire world (enemies, physics, enemy bullets, enemy animations, etc.)
        Time.timeScale = worldTimeScale;

        // Keep physics stable: shrink the fixed timestep proportionally
        // so FixedUpdate doesn't run 5x more often per real second
        Time.fixedDeltaTime = cachedFixedDeltaTime * worldTimeScale;
    }

    protected override void onDeactivate()
    {
        // Restore normal time
        Time.timeScale = cachedTimeScale;
        Time.fixedDeltaTime = cachedFixedDeltaTime;
    }
}