using System;
using Data.Mods;
using UnityEngine;

namespace Data.Wave
{
    [CreateAssetMenu(fileName = "New Wave", menuName = "Game/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Configuration")]
        public float durationSecs = 10f;
        public WaveSpawn[] spawnOrders;
        
        [Header("Rewards")]
        public int rewardChoiceCount   = 3;
        public ModDropEntry[] rewardPool;
    }

    [Serializable]
    public struct WaveSpawn
    {
        public GameObject enemyPrefab;
        public int count;
        [Range(0, 1)]
        [Tooltip("0 = Spawn at start, 1 = Spawn at end")]
        public float spawnTime;
    }

    [Serializable]
    public struct ModDropEntry
    {
        public WeaponMod mod;
        public int weight;
    } 
}