using UnityEngine;

public class heartbeatManager : MonoBehaviour
{
    public static heartbeatManager instance;

    [Header("BPM Settings")]
    [SerializeField] private int restingBPM = 60;
    [SerializeField] private int maxBPM = 180;
    [SerializeField] private int currentBPM;

    [Header("Stress Settings")]
    [SerializeField] private float currentStress = 0f;
    [SerializeField] private float maxStress = 100f;
    [SerializeField] private float stressDecayRate = 5f;

    [Header("Stress Change Values")]
    [SerializeField] private float shootStress = 2f;
    [SerializeField] private float damageStress = 25f;
    [SerializeField] private float nearMissStress = 10f;
    [SerializeField] private float killStressReduction = 10f;
    [SerializeField] private float waveStressReduction = 25f;

    private bool hasLost;

    void Awake()
    {
        // Allows other scripts to call heartbeatManager.instance
        instance = this;
    }

    void Start()
    {
        // Runtime value only. Designer values above are not overwritten.
        currentBPM = restingBPM;
    }

    void Update()
    {
        if (hasLost)
        {
            return;
        }

        decayStress();
        updateBPM();
        checkLoseCondition();
    }

    void decayStress()
    {
        // Stress slowly goes down over time if the player is not being pressured.
        currentStress -= stressDecayRate * Time.deltaTime;
        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
    }

    void updateBPM()
    {
        // Turns current stress into a BPM value.
        float stressPercent = currentStress / maxStress;
        currentBPM = Mathf.RoundToInt(Mathf.Lerp(restingBPM, maxBPM, stressPercent));

        // Sends the current BPM to the game manager UI.
        if (gameManager.instance != null)
        {
            gameManager.instance.updateHeartRate(currentBPM);
        }
    }

    void checkLoseCondition()
    {
        // Player loses when their BPM gets too high.
        if (currentBPM >= maxBPM)
        {
            hasLost = true;

            if (gameManager.instance != null)
            {
                gameManager.instance.stateLose();
            }
        }
    }

    public void addStress(float amount)
    {
        // Raises stress from combat events.
        currentStress += amount;
        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
    }

    public void reduceStress(float amount)
    {
        // Lowers stress from successful actions.
        currentStress -= amount;
        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
    }

    public void playerShot()
    {
        // Call from gunController when the player shoots.
        addStress(shootStress);
    }

    public void playerDamaged()
    {
        // Call when the player gets hit.
        // This replaces normal HP damage.
        addStress(damageStress);
    }

    public void nearMiss()
    {
        // Optional: call when a bullet barely misses the player.
        addStress(nearMissStress);
    }

    public void enemyKilled()
    {
        // Call when an enemy dies.
        reduceStress(killStressReduction);
    }

    public void waveCompleted()
    {
        // Call when a wave ends.
        reduceStress(waveStressReduction);
    }

    public void resetHeartbeat()
    {
        // Use when restarting the level.
        hasLost = false;
        currentStress = 0f;
        currentBPM = restingBPM;
    }

    public int getCurrentBPM()
    {
        return currentBPM;
    }

    public float getStressPercent()
    {
        return currentStress / maxStress;
    }
}