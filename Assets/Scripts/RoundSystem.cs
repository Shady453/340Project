using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RoundSystem : MonoBehaviour
{
    public UI UI;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(RunRounds());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
     [Header("Enemy")]
    public GameObject enemyPrefab;           // assign your DummyEnemy prefab
    public List<Transform> spawnPoints;      // drop in several transforms around the map

    [Header("Rounds")]
    public int startingRound = 1;
    public int currentRound { get; private set; }
    public float intermissionSeconds = 5f;   // time between rounds

    [Tooltip("Base enemies in round 1.")]
    public int baseEnemies = 1;

    [Tooltip("Extra enemies added each round.")]
    public int enemiesPerRound = 1;

    [Header("Scaling")]
    [Tooltip("Multiply enemy health each round: e.g., 1.20 = +20% per round")]
    public float healthMultiplierPerRound = 1.20f;

    [Tooltip("Optional: speed multiplier per round (1.0 = unchanged)")]
    public float speedMultiplierPerRound = 1.05f;

    [Header("Spawn Control")]
    [Tooltip("Cap how many are alive at once, to avoid huge bursts.")]
    public int concurrentAliveCap = 12;
    public float spawnInterval = 0.5f;

    public int AliveCount { get; private set; }

    bool _spawning;
    int _toSpawnThisRound;
    int _spawnedThisRound;

    void OnEnable()
    {
        DummyEnemy.Died += OnEnemyDied;     // subscribe to enemy death event
    }

    void OnDisable()
    {
        DummyEnemy.Died -= OnEnemyDied;
    }

    IEnumerator RunRounds()
    {
        currentRound = Mathf.Max(1, startingRound);

        while (true)
        {
            // Intermission
            yield return StartCoroutine(Intermission());

            // Start the round
            yield return StartCoroutine(StartRound(currentRound));

            // Wait until the round clears (no alive enemies)
            yield return new WaitUntil(() => AliveCount == 0 && _spawnedThisRound >= _toSpawnThisRound);

            print("Round " + (currentRound + 1) + ": " + _spawnedThisRound);
            currentRound++;
        }
    }

    IEnumerator Intermission()
    {
        // hook your UI here (e.g., show "Round X starting in ...")
        float t = intermissionSeconds;
        while (t > 0f)
        {
            // update UI countdown if you have one
            t -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator StartRound(int round)
    {
        // Compute wave size and reset counters
        _toSpawnThisRound     = baseEnemies + enemiesPerRound * (round - 1);
        _spawnedThisRound     = 0;
        _spawning             = true;

        // Difficulty scaling for this round
        float healthScale = Mathf.Pow(healthMultiplierPerRound, round - 1);
        float speedScale  = Mathf.Pow(speedMultiplierPerRound,  round - 1);

        // Spawn loop
        while (_spawning && _spawnedThisRound < _toSpawnThisRound)
        {
            // respect concurrent alive cap
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
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Count == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject e = Instantiate(enemyPrefab, sp.position, Quaternion.identity);

        // Scale difficulty on the spawned enemy
        var enemy = e.GetComponent<DummyEnemy>();
        if (enemy != null)
        {
            enemy.health = Mathf.Round(enemy.health * healthScale);

            // Optional: if your enemy has a mover, scale it too
            var mover = e.GetComponent<ZombieAiMove>();  // or whatever your mover script is
            if (mover != null)
            {
                mover.speed *= speedScale;               // make sure your mover exposes "speed"
            }
        }
    }

    void OnEnemyDied(DummyEnemy _)
    {
        print("Round: enemy died");
        AliveCount = Mathf.Max(0, AliveCount - 1);
        UI.ShowAliveCount(AliveCount);
    }
}
