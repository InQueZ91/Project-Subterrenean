using System;
using UnityEngine;

namespace Data.Wave
{
    [Serializable]
    public struct WaveSpawn
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
        [Range(0, 1)]
        [Tooltip("0 = Spawn at start, 1 = Spawn at end")]
        public float timing;
    }
}