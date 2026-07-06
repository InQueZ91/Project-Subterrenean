using UnityEngine;

namespace Runtime
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector3 knockback);
        void Die();
    }
}