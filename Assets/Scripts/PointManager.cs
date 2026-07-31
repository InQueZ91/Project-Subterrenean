using UnityEngine;
using UnityEngine.Events;

public class PointManager : MonoBehaviour
{
    [SerializeField] private int startPoint = 0;
    [SerializeField] private int noisePercentage = 3;
    [SerializeField] private float pointMultiplier = 1;

    private int _currentPoint;
        
    [Header("Events")]
    public UnityEvent<int, int> onPointChanged; // result, change
    public UnityEvent<int> onSpendFailed; // amount that was attempted
        
    // Singleton 
    public static PointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _currentPoint = startPoint;
        onPointChanged?.Invoke(_currentPoint, startPoint);
    }

    public void Gain(int amount)
    {
        var gained = CalculateGain(amount);
        _currentPoint += gained;
        onPointChanged?.Invoke(_currentPoint, gained);
    }
        
    public bool TrySpend(int amount)
    {
        if (amount > _currentPoint)
        {
            onSpendFailed?.Invoke(amount);
            return false;
        }
            
        _currentPoint -= amount;
        onPointChanged?.Invoke(_currentPoint, -amount);
        return true;
    }

    public void SetMultiplier(int multiplier)
    {
        pointMultiplier = Mathf.Max(1, multiplier);
    }

    private int CalculateGain(int amount)
    {
        var noiseRange = amount * noisePercentage / 100f;
        var noise = Random.Range(-noiseRange, noiseRange + 1);
        return (int)Mathf.Max(1, (amount + noise) * pointMultiplier);
    }
}