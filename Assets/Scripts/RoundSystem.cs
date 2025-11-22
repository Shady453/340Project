using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RoundSystem : MonoBehaviour
{
    public UI UI;

    // Global flag the entire game can check
    public static bool IsGameActive { get; private set; } = false;

    void Start()
    {
        
    }

    public void StartGame()
    {
        if (!IsGameActive)
        {
            IsGameActive = true;
            StartCoroutine(RunRounds());
        }
    }

    public void StopGame()
    {
        IsGameActive = false;  
        
    }

    

    [Header("Enemy")]
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints;

    [Header("Rounds")]
    public int startingRound = 1;
    public int currentRound { get; private set; }
    public float intermissionSeconds = 5f;
    public int baseEnemies = 1;
    public int enemiesPerRound = 1;

    [Header("Scaling")]
    public float healthMultiplierPerRound = 1.20f;
    public float speedMultiplierPerRound = 1.05f;

    [Header("Spawn Control")]
    public int concurrentAliveCap = 12;
    public float spawnInterval = 0.5f;

    public int AliveCount { get; private set; }

    bool _spawning;
    int _toSpawnThisRound;
    int _spawnedThisRound;

    void OnEnable()
    {
        DummyEnemy.Died += OnEnemyDied;
    }

    void OnDisable()
    {
        DummyEnemy.Died -= OnEnemyDied;
    }

    IEnumerator RunRounds()
    {
        currentRound = Mathf.Max(1, startingRound);

        // Show round 1 as soon as the game starts
        if (UI != null)
            UI.ShowRound(currentRound);

        while (IsGameActive)
        {
            // Intermission before the round (you can keep or shorten this)
            yield return StartCoroutine(Intermission());

            // Start the round
            yield return StartCoroutine(StartRound(currentRound));

            // Wait until all enemies for this round are dead
            yield return new WaitUntil(() => AliveCount == 0 && _spawnedThisRound >= _toSpawnThisRound);

            // Go to next round
            currentRound++;

            // Update the round text for the next round
            if (UI != null)
                UI.ShowRound(currentRound);
        }
    }


    IEnumerator Intermission()
    {
        float t = intermissionSeconds;
        while (t > 0f && IsGameActive)
        {
            t -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator StartRound(int round)
    {
        if (!IsGameActive) yield break;

        _toSpawnThisRound = baseEnemies + enemiesPerRound * (round - 1);
        _spawnedThisRound = 0;
        _spawning = true;

        float healthScale = Mathf.Pow(healthMultiplierPerRound, round - 1);
        float speedScale = Mathf.Pow(speedMultiplierPerRound, round - 1);

        while (_spawning && _spawnedThisRound < _toSpawnThisRound && IsGameActive)
        {
            if (AliveCount < concurrentAliveCap)
            {
                SpawnEnemy(healthScale, speedScale);
                _spawnedThisRound++;
                AliveCount++;
            }

            UI.ShowAliveCount(AliveCount);
            UI.ShowSpawnedThisRound(_spawnedThisRound);
            UI.ShowToSpawnThisRound(_toSpawnThisRound);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(float healthScale, float speedScale)
    {
        if (!IsGameActive) return;

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Count == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Vector3 spawnPos = sp.position;

        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out UnityEngine.AI.NavMeshHit hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"SpawnEnemy: No NavMesh near spawn point {sp.name}");
            return;
        }

        GameObject e = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        var agent = e.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && !agent.isOnNavMesh)
            agent.Warp(spawnPos);
    }

    void OnEnemyDied(DummyEnemy _)
    {
        AliveCount = Mathf.Max(0, AliveCount - 1);
        UI.ShowAliveCount(AliveCount);
    }
}
