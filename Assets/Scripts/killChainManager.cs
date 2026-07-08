using UnityEngine;

public class killChainManager : MonoBehaviour
{
    [SerializeField] float chainTimeLimit = 3f;

    int killChainCount = 0;
    float killChainTimer = 0f;


    // Update is called once per frame
    void Update()
    {
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
                Debug.Log("Double Kill!");
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
    }

    void ResetChain()
    {
        killChainCount = 0;
        killChainTimer = 0f;
        Debug.Log("Kill Chain Reset");
    }
}
