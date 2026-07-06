using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private Image fillDelta;
        [SerializeField] private float fillDeltaDelay = 1f;
        [SerializeField] private bool isEnemy = false;
        
        [Header("Colors")]
        [SerializeField] private Color playerFillColor;
        [SerializeField] private Color enemyFillColor;
        [SerializeField] private Color fillDeltaColor;
    
        private float _targetFill;
        
        private void Awake()
        {
            fill.color = isEnemy ? enemyFillColor : playerFillColor;
            fillDelta.color = fillDeltaColor;
            SetFill(1f);
        }

        public void SetFill(float value)
        {
            _targetFill = value;
            fill.fillAmount = value;
        }
        
        private void Update()
        {
            fillDelta.fillAmount = Mathf.Lerp(fillDelta.fillAmount, _targetFill, Time.deltaTime / fillDeltaDelay);
        }
    }
}