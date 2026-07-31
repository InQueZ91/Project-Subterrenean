using TMPro;
using UnityEngine;

namespace UI
{
    public class PointUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            scoreText.text = "CP:0";
        }

        public void OnPointChanged(int result, int change)
        {
            scoreText.text = $"CP:{result}";
        }
    }
}