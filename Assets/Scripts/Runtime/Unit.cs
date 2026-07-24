using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    public class Unit : MonoBehaviour, IDamageable
    {
        private float _maxHealth;
        public float CurrentHealth { get; private set; }
        private bool _isDead;
        
        // Events
        [Header("Events")]
        public UnityEvent<float> onHealthChanged;
        public UnityEvent<float, Vector3> onDamageTaken;
        public UnityEvent<float> onHealed;
        public UnityEvent onDied;
    
        public void Init(float maxHealth)
        {
            _maxHealth = maxHealth;
            CurrentHealth = _maxHealth;
        }
        
        public void UpdateStats(float newMaxHealth)
        {
            _maxHealth = newMaxHealth;
            CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth); // cap if new max is lower
            onHealthChanged?.Invoke(CurrentHealth / _maxHealth);
        }

        public void Heal(float amount)
        {
            CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth + amount);
            onHealthChanged?.Invoke(CurrentHealth / _maxHealth);
            onHealed?.Invoke(amount);
            
            if (CurrentHealth > 0f && _isDead) _isDead = false; 
        }
        
        public void TakeDamage(float amount, Vector3 knockback)
        {
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            onHealthChanged?.Invoke(CurrentHealth / _maxHealth);
            onDamageTaken?.Invoke(amount, knockback);
        
            if (CurrentHealth <= 0f) Die();
        }
        
        public void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            onDied?.Invoke();
        }
    }
}