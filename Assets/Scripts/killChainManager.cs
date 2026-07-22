using UnityEngine;
using TMPro;

public class killChainManager : MonoBehaviour
{
    public static killChainManager instance;

    [Header("Kill Chain Settings")]
    [SerializeField] private float chainTimeLimit;
    [SerializeField] private int killsPerStreakRoll;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI killChainCountUI;

    [Header("Runtime")]
    [SerializeField] private int killChainCount;
    [SerializeField] private float killChainTimer;

    // When true, RegisterKill will ignore incoming enemy deaths.
    // This prevents a nuke from generating additional killstreak rewards.
    private bool ignoreRegisteredKills;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Update()
    {
        updateKillChainUI();
        updateKillChainTimer();
    }

    void updateKillChainUI()
    {
        if (killChainCountUI != null)
        {
            killChainCountUI.text = "Killstreak: " + killChainCount;
        }
    }

    void updateKillChainTimer()
    {
        // The timer only runs while the player has an active chain.
        if (killChainCount <= 0)
            return;

        killChainTimer += Time.deltaTime;

        if (killChainTimer >= chainTimeLimit)
        {
            ResetChain();
        }
    }

    public void RegisterKill()
    {
        // Forced kills such as the Nuke still use the normal enemy death
        // sequence, but they should not increase the player's kill chain.
        if (ignoreRegisteredKills)
            return;

        killChainCount++;
        killChainTimer = 0f;

        Debug.Log("Kill Chain: " + killChainCount);

        switch (killChainCount)
        {
            case 2:
                Debug.Log("Double Kill!");
                break;

            case 3:
                Debug.Log("Triple Kill!");
                break;

            case 4:
                Debug.Log("Quadra Kill!");
                break;

            case 5:
                Debug.Log("Killing FRENZY!");
                break;
        }

        // Every configured number of kills, attempt to award
        // the player a random killstreak.
        if (killsPerStreakRoll > 0 &&
            killChainCount % killsPerStreakRoll == 0)
        {
            if (killstreakManager.instance != null)
            {
                killstreakManager.instance.tryRoll();
            }
        }
    }

    public void SetIgnoreRegisteredKills(bool shouldIgnore)
    {
        ignoreRegisteredKills = shouldIgnore;
    }

    public void ResetChain()
    {
        killChainCount = 0;
        killChainTimer = 0f;

        Debug.Log("Kill Chain Reset");
    }

    public int GetKillChainCount()
    {
        return killChainCount;
    }
}
