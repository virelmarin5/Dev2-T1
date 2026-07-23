/*
 * Script: WaveManager
 *
 * Description:
 * Central wave coordinator. Does NOT spawn enemies itself - each
 * spawnPoint is fully individual (own prefabs, percentages, pacing,
 * and difficulty scaling). WaveManager just owns the wave number and
 * the countdown between waves, and keeps every spawnPoint in sync:
 * the next wave will not begin until every registered spawnPoint has
 * finished spawning AND every enemy from every spawnPoint is dead.
 *
 * Responsibilities:
 * - Automatically start the first wave when the level begins
 * - Wait between waves (real-time, unaffected by Time.timeScale),
 *   then tell every spawnPoint to begin the new wave
 * - Track total enemies alive across all spawn points
 * - Only complete a wave once ALL spawn points are done spawning and
 *   ALL of their enemies are dead
 * - Notify HeartbeatManager when enemies die or waves end
 * - Notify GameManager when all waves are completed
 *
 * Interacts With:
 * - spawnPoint (one or more, individually configured)
 * - heartbeatManager
 * - gameManager
 * - waveLightController
 * - audioManager
 */

using System.Collections.Generic;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;

    [Header("Wave Settings")]
    [SerializeField] private int currentWave;
    [SerializeField] private int maxWaves;
    [SerializeField] private float timeBetweenWaves;

    [Header("Runtime")]
    [SerializeField] private int enemiesAlive;
    [SerializeField] private bool waveInProgress;

    private bool waitingForNextWave;
    private float waveTimer;

    private List<spawnPoint> spawners = new List<spawnPoint>();
    private int spawnersStillSpawning;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        queueNextWave();
    }

    void Update()
    {
        if (waitingForNextWave)
        {
            // Unscaled so the countdown is always real seconds, regardless
            // of Time.timeScale (slow-mo, hit-stop, etc.).
            waveTimer += Time.unscaledDeltaTime;

            if (waveTimer >= timeBetweenWaves)
            {
                waitingForNextWave = false;
                startWave();
            }
        }
    }

    // Called once by each spawnPoint (in its own Start) so waveManager
    // knows it exists and should be included in wave coordination.
    public void RegisterSpawner(spawnPoint sp)
    {
        if (!spawners.Contains(sp))
        {
            spawners.Add(sp);
        }
    }

    public void UnregisterSpawner(spawnPoint sp)
    {
        spawners.Remove(sp);
    }

    void queueNextWave()
    {
        currentWave++;

        if (currentWave > maxWaves)
        {
            playerWins();
            return;
        }

        if (waveLightController.instance != null)
        {
            waveLightController.instance.FlashWarningLights(timeBetweenWaves);
        }

        if (audioManager.instance != null)
        {
            audioManager.instance.playRoundTransitionMusic();
        }

        waveTimer = 0f;
        waitingForNextWave = true;
    }

    void startWave()
    {
        if (audioManager.instance != null)
        {
            audioManager.instance.stopMusic();
        }

        waveInProgress = true;
        enemiesAlive = 0;
        spawnersStillSpawning = spawners.Count;

        // Every spawn point runs its own count/pacing/prefab logic and just
        // reports back how many enemies it committed to spawning this wave.
        foreach (spawnPoint sp in spawners)
        {
            enemiesAlive += sp.BeginWave(currentWave);
        }

        // Edge case: no spawn points registered, or every spawn point had
        // nothing to spawn this wave - don't get stuck forever waiting.
        if (spawners.Count == 0 || (spawnersStillSpawning <= 0 && enemiesAlive <= 0))
        {
            completeWave();
        }
    }

    // Called by a spawnPoint once it has finished spawning its quota for the
    // current wave (this means "done spawning", not "all its enemies died").
    public void SpawnerFinishedSpawning(spawnPoint sp)
    {
        spawnersStillSpawning--;

        if (spawnersStillSpawning < 0)
        {
            spawnersStillSpawning = 0;
        }

        if (spawnersStillSpawning <= 0 && enemiesAlive <= 0)
        {
            completeWave();
        }
    }

    public void enemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }

        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.enemyKilled();
        }

        if (spawnersStillSpawning <= 0 && enemiesAlive <= 0)
        {
            completeWave();
        }
    }

    void completeWave()
    {
        if (!waveInProgress)
        {
            return;
        }

        waveInProgress = false;

        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.waveCompleted();
        }

        queueNextWave();
    }

    void playerWins()
    {
        if (gameManager.instance != null)
        {
            // Add this once your gameManager has a win menu.
            // gameManager.instance.stateWin();
        }
    }

    public int getCurrentWave()
    {
        return currentWave;
    }

    public int getEnemiesAlive()
    {
        return enemiesAlive;
    }

    public bool isWaveInProgress()
    {
        return waveInProgress;
    }

    public bool isWaitingForNextWave()
    {
        return waitingForNextWave;
    }

    public int getSecondsUntilNextWave()
    {
        float remaining = timeBetweenWaves - waveTimer;
        return Mathf.Max(0, Mathf.CeilToInt(remaining));
    }
}















































// /*
//  * Script: WaveManager
//  * Author: Devin Childs
//  *
//  * Description:
//  * Controls enemy wave spawning and difficulty progression.
//  * Each wave increases the total number of enemies by a percentage.
//  * Enemy types are chosen by percentage so most enemies are ranged,
//  * some are basic melee enemies, and the fewest are heavy enemies.
//  *
//  * Responsibilities:
//  * - Start new waves
//  * - Spawn enemies at spawn points
//  * - Increase enemy count by percentage each wave
//  * - Spawn weighted enemy types
//  * - Track enemies alive
//  * - Notify HeartbeatManager when enemies die or waves end
//  * - Notify GameManager when all waves are completed
//  *
//  * Interacts With:
//  * - EnemyBase
//  * - BasicEnemy
//  * - RangedEnemy
//  * - heavyEnemy
//  * - heartbeatManager
//  * - gameManager
//  *
//  * Last Updated:
//  * Prototype 1
//  */

// using System.Collections;
// using UnityEngine;

// public class waveManager : MonoBehaviour
// {
//     public static waveManager instance;

//     [Header("Wave Settings")]
//     [SerializeField] public int currentWave;
//     [SerializeField] private int maxWaves;
//     [SerializeField] private int startingEnemies;

//     [Tooltip("Example: 25 means each wave has 25% more enemies than the last.")]
//     [SerializeField] private float enemyIncreasePercent;

//     [SerializeField] private float timeBetweenWaves;
//     [SerializeField] private float timeBetweenSpawns;

//     [Header("Enemy Prefabs")]
//     [SerializeField] private GameObject basicEnemyPrefab;
//     [SerializeField] private GameObject rangedEnemyPrefab;
//     [SerializeField] private GameObject heavyEnemyPrefab;

//     [Header("Enemy Spawn Percentages")]
//     [Tooltip("Largest percent. Example: 60")]
//     [SerializeField] private float rangedEnemyPercent;

//     [Tooltip("Middle percent. Example: 30")]
//     [SerializeField] private float basicEnemyPercent;

//     [Tooltip("Lowest percent. Example: 10")]
//     [SerializeField] private float heavyEnemyPercent;

//     [Header("Spawn Points")]
//     [SerializeField] private Transform[] spawnPoints;

//     [Header("Runtime")]
//     [SerializeField] private int enemiesAlive;
//     [SerializeField] private int enemiesSpawnedThisWave;
//     [SerializeField] private bool waveInProgress;

//     private bool isSpawning;

//     void Awake()
//     {
//         if (instance != null && instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         instance = this;
//     }

//     void Start()
//     {
//         StartCoroutine(beginNextWave());
//     }

//     IEnumerator beginNextWave()
//     {
//         // Flash the warning lights while waiting for the next wave.
//         if (waveLightController.instance != null)
//         {
//             waveLightController.instance.FlashWarningLights(timeBetweenWaves);
//         }

//         if (audioManager.instance != null)
//         {
//             audioManager.instance.playRoundTransitionMusic();
//         }

//         // Wait for the warning sequence to finish.
//         yield return new WaitForSeconds(timeBetweenWaves);

//         if (audioManager.instance != null) audioManager.instance.stopMusic();

//         currentWave++;

//         if (currentWave > maxWaves)
//         {
//             playerWins();
//             yield break;
//         }

//         waveInProgress = true;
//         isSpawning = true;
//         enemiesSpawnedThisWave = 0;

//         int enemiesToSpawn = getEnemyCountForWave();

//         enemiesAlive = enemiesToSpawn;

//         //Debug.Log("Wave " + currentWave + " started. Enemies: " + enemiesToSpawn);

//         for (int i = 0; i < enemiesToSpawn; i++)
//         {
//             spawnEnemy();
//             enemiesSpawnedThisWave++;

//             yield return new WaitForSeconds(timeBetweenSpawns);
//         }

//         isSpawning = false;
//     }

//     void spawnEnemy()
//     {
//         if (spawnPoints == null || spawnPoints.Length == 0)
//         {
//             //Debug.LogWarning("WaveManager has no spawn points assigned.");
//             return;
//         }

//         GameObject enemyToSpawn = chooseEnemyPrefab();

//         if (enemyToSpawn == null)
//         {
//             //Debug.LogWarning("WaveManager is missing an enemy prefab.");
//             return;
//         }

//         Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

//         GameObject enemyObj = Instantiate(enemyToSpawn, chosenSpawn.position, chosenSpawn.rotation);
//         EnemyBase eb = enemyObj.GetComponent<EnemyBase>();
//     }

//     GameObject chooseEnemyPrefab()
//     {
//         float totalPercent = rangedEnemyPercent + basicEnemyPercent + heavyEnemyPercent;

//         if (totalPercent <= 0)
//         {
//             //Debug.LogWarning("Enemy spawn percentages are not set.");
//             return null;
//         }

//         float randomValue = Random.Range(0, totalPercent);

//         if (randomValue < rangedEnemyPercent)
//         {
//             return rangedEnemyPrefab;
//         }

//         randomValue -= rangedEnemyPercent;

//         if (randomValue < basicEnemyPercent)
//         {
//             return basicEnemyPrefab;
//         }

//         return heavyEnemyPrefab;
//     }

//     int getEnemyCountForWave()
//     {
//         /*
//          * Example with:
//          * startingEnemies = 5
//          * enemyIncreasePercent = 25
//          *
//          * Wave 1 = 5
//          * Wave 2 = 7
//          * Wave 3 = 8
//          * Wave 4 = 10
//          */

//         float increaseMultiplier = 1 + (enemyIncreasePercent / 100f);

//         float enemyCount = startingEnemies * Mathf.Pow(increaseMultiplier, currentWave - 1);

//         return Mathf.CeilToInt(enemyCount);
//     }

//     public void enemyKilled()
//     {
//         enemiesAlive--;

//         if (enemiesAlive < 0)
//         {
//             enemiesAlive = 0;
//         }

//         if (heartbeatManager.instance != null)
//         {
//             heartbeatManager.instance.enemyKilled();
//         }

//         //Debug.Log("Enemy killed. Enemies alive: " + enemiesAlive);

//         if (!isSpawning && enemiesAlive <= 0)
//         {
//             completeWave();
//         }
//     }

//     void completeWave()
//     {
//         if (!waveInProgress)
//         {
//             return;
//         }

//         waveInProgress = false;

//         //Debug.Log("Wave " + currentWave + " completed.");

//         if (heartbeatManager.instance != null)
//         {
//             heartbeatManager.instance.waveCompleted();
//         }

//         StartCoroutine(beginNextWave());
//     }

//     void playerWins()
//     {
//         //Debug.Log("All waves completed. Player wins.");

//         if (gameManager.instance != null)
//         {
//             // Add this once your gameManager has a win menu.
//             // gameManager.instance.stateWin();
//         }
//     }

//     public int getCurrentWave()
//     {
//         return currentWave;
//     }

//     public int getEnemiesAlive()
//     {
//         return enemiesAlive;
//     }

//     public bool isWaveInProgress()
//     {
//         return waveInProgress;
//     }
// }
