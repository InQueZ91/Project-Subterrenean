using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            scoreText.text = "0";
        }

        private void OnScoreChanged(int newScore)
        {
            scoreText.text = newScore.ToString();
        }
    }
}