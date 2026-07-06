using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using Data.Stats;
using Runtime.Spawners;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private List<SpawnPoint> spawnPoints;

    [Header("Events")] 
    public UnityEvent<int> onWaveStarted;
    public UnityEvent<int, int> onEnemySpawned; // spawned, total  
    public UnityEvent onWaveCleared;
    public UnityEvent onAllWavesCleared;
    
    // State
    private int _currentWaveIndex;
    private int _enemiesAlive;
    private int _enemiesSpawned;

    private void Start()
    {
        StartCoroutine(RunWave(_currentWaveIndex));
    }

    private IEnumerator RunWave(int index)
    {
        if (index >= waves.Count)
        {
            onAllWavesCleared?.Invoke();
            yield break;
        }
        
        var wave = waves[index];
        
        // Countdown before wave
        yield return new WaitForSeconds(wave.timeBeforeWave);
        
        _enemiesAlive = 0;
        _enemiesSpawned = 0;
        onWaveStarted?.Invoke(index + 1);
        
        // Trickle spawn
        while (_enemiesSpawned < wave.enemyCount)
        {
            SpawnEnemy(wave);
            _enemiesSpawned++;
            onEnemySpawned?.Invoke(_enemiesSpawned, wave.enemyCount);

            if (_enemiesSpawned < wave.enemyCount)
                yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // Wait until all enemies are dead
        while (_enemiesAlive > 0) 
            yield return null;
        
        onWaveCleared?.Invoke();
        _currentWaveIndex++;
        StartCoroutine(RunWave(_currentWaveIndex));
    }

    private void SpawnEnemy(WaveData wave)
    {
        if (spawnPoints.Count == 0) return;
        
        // Pick a random spawn point
        var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        
        // Spawn enemy
        var go = Instantiate(wave.enemyPrefab, point.transform.position, Quaternion.identity);
        go.name = $"Enemy {_currentWaveIndex}-{_enemiesSpawned + 1}";
        go.transform.SetParent(transform);
        var enemy = go.GetComponent<EnemyController>();
        enemy.onDied.AddListener(OnEnemyDied);
        
        _enemiesAlive++;
    }

    private void OnEnemyDied(RewardStats rewards, Vector3 spawnPosition)
    {
        _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);
        
        ScoreManager.Instance.AddScore(rewards.scoreValue);
        LootSpawner.Instance.SpawnFromDropTable(rewards.drops, spawnPosition);
    }
}