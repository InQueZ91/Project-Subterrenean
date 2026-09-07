using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data.Stats;
using Data.Wave;
using Runtime.Spawners;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Vector3 playerSpawnPosition;
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private List<SpawnPoint> spawnPoints;
    [SerializeField] private bool isActivated = false;

    [Header("Events")] 
    public UnityEvent<int> onWaveStarted;
    public UnityEvent<int, int> onEnemySpawned; // spawned, total  
    public UnityEvent onWaveCleared;
    public UnityEvent onAllWavesCleared;
    
    // State
    private int _currentWaveIndex;
    private int _enemiesAlive;
    private int _enemiesSpawned;

    // Singleton
    public static WaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        StartCoroutine(RunWave(_currentWaveIndex));
    }
    
    // Methods
    public void StartNextWave()
    {
        if (_currentWaveIndex >= waves.Count) return;
        
        _currentWaveIndex++;
        StartCoroutine(RunWave(_currentWaveIndex));
    }

    private IEnumerator RunWave(int index)
    {
        if (!isActivated) yield break;
        
        // Check if all waves are cleared
        if (index >= waves.Count)
        {
            onAllWavesCleared?.Invoke();
            yield break;
        }
        
        var wave = waves[index];
        
        // Countdown before wave
        yield return new WaitForSeconds(wave.timeBeforeWave);
        Debug.Log($"Wave {_currentWaveIndex} started!");
        
        // Reset state
        _enemiesAlive = 0;
        _enemiesSpawned = 0;
        onWaveStarted?.Invoke(index + 1);
        
        // Spawn enemies
        var waveStartTime = Time.time;
        var waveEndTime = waveStartTime + wave.waveDurationSecs;
        var order = 0;
        while (Time.time < waveEndTime)
        {
            // Wait for wave reach end time
            if (order >= wave.spawns.Length)
            {
                yield return null;
                continue;
            }
            
            var spawn = wave.spawns[order];
            var timing = waveStartTime + spawn.timing * wave.waveDurationSecs;
            if (Time.time > timing)
            {
                var spawnedThisOrder = 0;
                while (spawnedThisOrder < spawn.count)
                {
                    SpawnEnemy(spawn.enemyPrefab);
                    _enemiesSpawned++;
                    spawnedThisOrder++;
                    onEnemySpawned?.Invoke(_enemiesSpawned, spawn.count);
                    
                    if (spawnedThisOrder < spawn.count)
                        yield return new WaitForSeconds(spawn.spawnInterval);
                }
                
                order++;
            }
            else
            {
                yield return null;
            }
        }
        
        // Wait until all enemies are dead
        while (_enemiesAlive > 0) 
            yield return null;
        
        onWaveCleared?.Invoke();
        OnWaveCleared(wave);
    }

    private void OnWaveCleared(WaveData wave)
    {
        Debug.Log($"Wave {_currentWaveIndex} cleared!");
        // Spawn rewards at center of the map or spawn points
        LootSpawner.Instance.SpawnFromDropTable(wave.itemRewardPool, playerSpawnPosition);
        
        // Start weapon mod rewards
        // Another singleton but not implemented yet

        StartNextWave();
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available!");
            return;
        }
        
        var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        
        var go = Instantiate(prefab, point.transform.position, Quaternion.identity);
        go.name = $"Enemy {_currentWaveIndex}-{_enemiesSpawned + 1}";
        go.transform.SetParent(transform);
        
        var enemy = go.GetComponent<IEnemy>();
        enemy?.OnDied.AddListener(OnEnemyDied);

        _enemiesAlive++;
    }

    private void OnEnemyDied(RewardStats rewards, Vector3 spawnPosition)
    {
        _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);
        
        // PointManager.Instance.Gain(rewards.scoreValue);
        
        // Spawn crush object
        LootSpawner.Instance.SpawnFromDropTable(rewards.drops, spawnPosition);
    }
}