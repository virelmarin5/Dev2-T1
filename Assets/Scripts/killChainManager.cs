using UnityEngine;
using TMPro;
public class killChainManager : MonoBehaviour
{
    public static killChainManager instance;
    [SerializeField] float chainTimeLimit = 3f;
    [SerializeField] TextMeshProUGUI killChainCountUI;

    [SerializeField] int killsPerStreakRoll = 3;

    int killChainCount = 0;
    float killChainTimer = 0f;

    public bool activatePlayershield = false;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        killChainCountUI.text = "Killstreak: " + killChainCount;
        //only counts down if a chain is active
        if (killChainCount > 0)
        {
            killChainTimer += Time.deltaTime;

            if (killChainTimer >= chainTimeLimit)
            {
                ResetChain();
            }

        }

    }

    public void RegisterKill()
    {
        killChainCount++;
        killChainTimer = 0f;

        Debug.Log("Kill Chain: " + killChainCount);

        switch (killChainCount)
        {
            case 2:
                activatePlayershield = true;
                break;
            case 3:
                Debug.Log("Triple Kill!");
                break;
            case 4:
                Debug.Log("Quadra Kill");
                break;
            case 5:
                Debug.Log("Killing FRENZY!!");
                break;
        }

        if (killsPerStreakRoll > 0 && killChainCount % killsPerStreakRoll == 0)
        {
            if (killstreakManager.instance != null)
            {
                killstreakManager.instance.tryRoll();
            }
        }
    }

    void ResetChain()
    {
        killChainCount = 0;
        killChainTimer = 0f;
        Debug.Log("Kill Chain Reset");
    }
}
