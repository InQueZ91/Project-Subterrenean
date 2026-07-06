using TMPro;
using UnityEngine;

namespace UI
{
    public class WaveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI waveText;
    
        private void OnEnable()
        {
            waveText.text = "0";
        }

        public void OnEnemySpawned(int spawnedCount, int totalCount)
        {
            waveText.text = $"{totalCount - spawnedCount}";
        }
    }
}
