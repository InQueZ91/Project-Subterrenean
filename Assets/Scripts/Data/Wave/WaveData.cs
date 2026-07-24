using UnityEngine;

namespace Data.Wave
{
    [CreateAssetMenu(fileName = "New Wave", menuName = "Game/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Configuration")]
        public float timeBeforeWave = 3f;
        public float waveDurationSecs = 10f;
        public WaveSpawn[] spawns;
        
        [Header("Rewards")]
        public DropData[] itemRewardPool;
    }
}