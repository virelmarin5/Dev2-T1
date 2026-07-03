using UnityEngine;

public class timeManager : MonoBehaviour
{

    public static timeManager instance;

    [SerializeField] float defaultTimeScale;
    [SerializeField] float currentTimeScale;

    [SerializeField] bool update;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = defaultTimeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            Time.timeScale = currentTimeScale;
            update = false;
        }
    }

    public void setTimeScale(float newTimeScale)
    {
        currentTimeScale = newTimeScale;
    }
}
