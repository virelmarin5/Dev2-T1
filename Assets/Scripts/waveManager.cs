/*
 * Script: WaveManager
 * Author: Devin Childs
 *
 * Description:
 * Controls the game's wave progression system.
 * Responsible for spawning enemies, increasing difficulty,
 * tracking remaining enemies, and determining when the
 * player has completed all waves.
 *
 * Responsibilities:
 * - Start new waves
 * - Spawn enemies
 * - Increase enemy count each wave
 * - Track enemies remaining
 * - Notify HeartbeatManager when a wave ends
 * - Notify GameManager when all waves are completed
 *
 * Interacts With:
 * - EnemyBase
 * - heartbeatManager
 * - gameManager
 *
 * Last Updated:
 * Prototype 1
 */

using System.Collections;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;

    [Header("Wave Settings")]
    [SerializeField] private int currentWave;
    [SerializeField] private int maxWaves;

    [SerializeField] private int startingEnemies;
    [SerializeField] private int enemiesAddedPerWave;

    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float timeBetweenSpawns;

    [Header("Enemy References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Runtime")]
    [SerializeField] private int enemiesAlive;
    [SerializeField] private bool waveInProgress;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(beginNextWave());
    }

    IEnumerator beginNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;

        // Player has survived every wave.
        if (currentWave > maxWaves)
        {
            playerWins();
            yield break;
        }

        waveInProgress = true;

        int enemiesToSpawn = startingEnemies +
                             ((currentWave - 1) * enemiesAddedPerWave);

        enemiesAlive = enemiesToSpawn;

        Debug.Log("Wave " + currentWave + " Started");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            spawnEnemy();

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void spawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
            return;

        GameObject enemy =
            enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Transform spawn =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemy, spawn.position, spawn.rotation);
    }

    // Called by an enemy when it dies.
    public void enemyKilled()
    {
        enemiesAlive--;

        // Reduce player stress after each kill.
        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.enemyKilled();
        }

        if (enemiesAlive <= 0)
        {
            completeWave();
        }
    }

    void completeWave()
    {
        waveInProgress = false;

        Debug.Log("Wave " + currentWave + " Complete");

        // Completing a wave calms the player down.
        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.waveCompleted();
        }

        StartCoroutine(beginNextWave());
    }

    void playerWins()
    {
        Debug.Log("Player Wins!");

        if (gameManager.instance != null)
        {
            // Uncomment once implemented.
            // gameManager.instance.stateWin();
        }
    }

    //=========================
    // Getters
    //=========================

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
}
