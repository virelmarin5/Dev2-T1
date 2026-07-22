using UnityEngine;

public class timeManager : MonoBehaviour
{

    public static timeManager instance;


    [Header("Time Scale Range")]
    [SerializeField] float minTimeScale;
    [SerializeField] float maxTimeScale;
    [SerializeField] float moveMaxTimeScale;

    [SerializeField] float timeScaleSmoothing;

    [Header("Heartbeat Influence")]
    [SerializeField] float bpmInfluence;

    float currentTimeScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        Time.timeScale = minTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        currentTimeScale = Time.timeScale;
    }

    private void Update()
    {
        if (gameManager.instance == null || gameManager.instance.isPaused) return;

        // Get the player's speed percentage and calculate the target time scale based on it.
        float playerSpeedPercent = gameManager.instance.playerScript.getSpeedPercent();
        float target = Mathf.Lerp(minTimeScale, moveMaxTimeScale, playerSpeedPercent);


        // If the heartbeatManager instance exists, get the stress percentage and adjust the target time scale accordingly.
        if (heartbeatManager.instance != null)
        {
            float stressPercent = heartbeatManager.instance.getStressPercent();
            target += stressPercent * bpmInfluence;
            target = Mathf.Clamp(target, minTimeScale, maxTimeScale);
        }

        // Smoothly interpolate the current time scale towards the target time scale using Mathf.Lerp for a gradual transition.
        float smoothed = Mathf.Lerp(Time.timeScale, target, timeScaleSmoothing * Time.unscaledDeltaTime);
        setTimeScale(smoothed);
        
    }

    public void setTimeScale(float newTimeScale)
    {
        if (gameManager.instance != null && gameManager.instance.isPaused) return;

        Time.timeScale = newTimeScale;
        // Adjust the fixedDeltaTime based on the new time scale for consistent physics updates like bullets and player movement. This ensures that physics calculations remain stable regardless of the time scale.
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        currentTimeScale = Time.timeScale;
    }


    public float getTimeScale()
    {
        return Time.timeScale;
    }

    public void pauseTime()
    {
        Time.timeScale = 0;
    }
    
    public void unpauseTime()
    {
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
