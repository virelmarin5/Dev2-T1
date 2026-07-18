using System.Collections;
using TMPro;
using UnityEngine;

/*
 * created by Mark Fittante
 * 
 * Connects to any script that results in enmey death
 * 
 * Depending on which enemy will show different scores
 * 
 * 
 */





public class killScoreUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] TextMeshProUGUI pauseKills;

    [Header("UI Effects")]
    [SerializeField] float fadeTime = 0.75f;
    [SerializeField] float displayTime = 2f;
    [SerializeField] float moveBackTime = 0.5f;
    [SerializeField] ParticleSystem burstEffect;

    int currentKill;
    //int currentScore;
    int enemiesAlive;
    int previousEnemiesAlive;
    int previousWave;


    // WAVE EFFECTS
    Vector2 waveCounterOrigPos;
    // WAVE EFFECTS 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        currentKill = 0;
        //currentScore = 0;

        previousEnemiesAlive = waveManager.instance.getEnemiesAlive();
        previousWave = waveManager.instance.getCurrentWave();

        waveCounterOrigPos = waveCounter.rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        int currentWave = waveManager.instance.getCurrentWave();
        enemiesAlive = waveManager.instance.getEnemiesAlive();

        if (currentWave != previousWave)
        {
            previousWave = currentWave;
            previousEnemiesAlive = enemiesAlive;

            StartCoroutine(waveEffects(currentWave));
        }
        else if (enemiesAlive < previousEnemiesAlive)
        {
            currentKill += previousEnemiesAlive - enemiesAlive;
        }

        previousEnemiesAlive = enemiesAlive;

        killCounter.text = "Kills: " + currentKill;
        pauseKills.text = "Kills: " + currentKill;
        //waveCounter.text = "Wave: " + currentWave;
    }

    IEnumerator waveEffects(int wave)
    {
        if (burstEffect != null)
        {
            burstEffect.Play();
        }
        
        waveCounter.text = $"Wave {wave}";

        Color c = waveCounter.color;
        c.a = 0f;
        waveCounter.color = c;
        //
        Canvas canvas = waveCounter.canvas;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;


        Vector2 centerPosition = new Vector2(0, -canvasHeight / 2f);

        waveCounter.transform.localScale = Vector3.one * 0.6f;



        float timer = 0f;

        while (timer < fadeTime)
        {
            timer+= Time.deltaTime;
            float t = timer / fadeTime;

            c.a = Mathf.Lerp(0f,1f,t);
            waveCounter.color = c;

            waveCounter.transform.localScale = Vector3.Lerp(Vector3.one * 0.6f, Vector3.one, t);
            waveCounter.rectTransform.anchoredPosition = Vector2.Lerp(waveCounterOrigPos, centerPosition, t);


            yield return null;
        }

        c.a = 1f;
        waveCounter.color = c;
        waveCounter.transform.localScale = Vector3.one;
        waveCounter.rectTransform.anchoredPosition = centerPosition;

        yield return new WaitForSeconds(displayTime);

        timer = 0f;
        while (timer < moveBackTime)
        {
            timer += Time.deltaTime;
            float t = timer / moveBackTime; 

            c.a = Mathf.Lerp(1f, 0f, t);
            waveCounter.color = c;

            waveCounter.rectTransform.anchoredPosition = Vector2.Lerp(centerPosition, waveCounterOrigPos, t);

            yield return null;

        }

        c.a = 1f;
        waveCounter.color = c;
        waveCounter.rectTransform.anchoredPosition = waveCounterOrigPos;
        
    }

    public int getKillCount()
    {
        return currentKill;
    }
}
