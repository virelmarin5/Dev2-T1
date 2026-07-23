using UnityEngine;
using System.Collections;

public abstract class killstreakBase : MonoBehaviour
{

    [Header("Killstreak Info")]
    [SerializeField] protected string killstreakName;
    [SerializeField] protected float duration;

    public bool isActive { get; private set; }

    Coroutine runRoutine;

    public string GetKillstreakName()
    {
        return killstreakName;
    }

    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        //Debug.Log(killstreakName + " activated!");
        onActivate();

        if (duration <= 0f)
        {
            endStreak();
        }
        else
        {
            runRoutine = StartCoroutine(runTimer());
        }
    }

    public void Deactivate()
    {
        if (!isActive) return;

        if (runRoutine != null)
        {
            StopCoroutine(runRoutine);
            runRoutine = null;
        }

        endStreak();
    }

    IEnumerator runTimer()
    {
        float elapse = 0f;

        while (elapse < duration)
        {
            yield return null;

            if (gameManager.instance != null && gameManager.instance.isPaused)
            {
                continue;
            }

            elapse += Time.unscaledDeltaTime;
            onTick(Time.unscaledDeltaTime);
        }
        runRoutine = null;
        endStreak();
    }

    void endStreak()
    {
        isActive = false;
        onDeactivate();
        //Debug.Log("Killstreak ended: " + killstreakName);

        if (killstreakManager.instance != null)
        {
            killstreakManager.instance.streakEnded(this);
        }
    }

    // Overrides

    protected abstract void onActivate();

    protected virtual void onTick(float unscaledDeltaTime) { }

    protected abstract void onDeactivate();
    
}
