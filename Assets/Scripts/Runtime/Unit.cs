using Data.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    public class Unit : MonoBehaviour, IDamageable
    {
        private UnitStats _stats;
        public float CurrentHealth { get; private set; }
        private bool _isDead;
        
        // Events
        [Header("Events")]
        public UnityEvent<float> onHealthChanged;
        public UnityEvent<float, Vector3> onDamageTaken;
        public UnityEvent<float> onHealed;
        public UnityEvent onDied;
    
        public void Init(UnitStats stats)
        {
            _stats = stats;
            CurrentHealth = _stats.maxHealth;
        }

        public void Heal(float amount)
        {
            CurrentHealth = Mathf.Min(_stats.maxHealth, CurrentHealth + amount);
            onHealthChanged?.Invoke(CurrentHealth / _stats.maxHealth);
            onHealed?.Invoke(amount);
        }
        
        // Damage
        public void TakeDamage(float amount, Vector3 knockback)
        {
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            onHealthChanged?.Invoke(CurrentHealth / _stats.maxHealth);
            onDamageTaken?.Invoke(amount, knockback);
        
            if (CurrentHealth <= 0f) Die();
        }
        public void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            onDied?.Invoke();
            Destroy(gameObject, 0.1f);
        }
    }
}