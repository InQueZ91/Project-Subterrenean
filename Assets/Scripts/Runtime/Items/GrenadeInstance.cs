using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrenadeInstance : MonoBehaviour
    {
        // Configuration
        private DamageStats _damageStats;
        private HitEffect[] _hitEffects;
        private float _fuseTime;
        private GameObject _owner;
        
        // References
        private Rigidbody _rb;
        
        // State
        private float _detonateTime;

        public void Init(
            GameObject owner,
            DamageStats damageStats,
            HitEffect[] hitEffects,
            float fuseTime,
            Vector3 throwVelocity)
        {
            _owner = owner;
            _damageStats = damageStats;
            _hitEffects = hitEffects;
            _fuseTime = fuseTime;
            
            _rb = GetComponent<Rigidbody>();
            _rb.linearVelocity = throwVelocity;
            
            _detonateTime = Time.time + _fuseTime;
        }

        private void Update()
        {
            if (Time.time >= _detonateTime) Detonate();
        }

        private void Detonate()
        {
            if (_hitEffects != null)
            {
                var hitContext = new HitContext
                {
                    point = transform.position,
                    direction = Vector3.up,
                    victim = null,
                    owner = _owner,
                    stats = _damageStats
                };
 
                foreach (var effect in _hitEffects)
                    effect.Apply(hitContext);
            }
 
            Destroy(gameObject);
        }
    }
}