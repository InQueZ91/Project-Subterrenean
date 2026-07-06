using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Wave", menuName = "Game/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Settings")] 
        public int enemyCount = 10;
        public float spawnInterval = 2f;
        public float timeBeforeWave = 3f;
        
        public GameObject enemyPrefab;
    }
}