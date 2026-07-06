using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public UnityEvent<int> onScoreChanged;

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void AddScore(int amount)
    {
        Score += amount;
        onScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        onScoreChanged?.Invoke(Score);
    }
}