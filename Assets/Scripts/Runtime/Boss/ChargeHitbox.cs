using Data.CombatMove;
using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Boss
{
    /// <summary>
    /// Trailing hitbox for a Charge move. Unlike HitboxSpawner's stateless,
    /// single-call overlap check, this needs continuous trigger detection
    /// while the boss is physically moving — a fundamentally different
    /// shape (event-driven OnTriggerEnter vs polled OverlapSphere), so it's
    /// its own small script rather than a HitboxSpawner variant.
    ///
    /// Spawned and positioned by BossController for the duration of a charge,
    /// destroyed/deactivated on arrival.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class ChargeHitbox : MonoBehaviour
    {
        private HitEffect[] _hitEffects;
        private DamageStats _damageStats;
        private LayerMask _obstacleDestroyMask;
        private GameObject _owner;

        private SphereCollider _collider;
        
        private bool _isActive;
        
        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }
        
        public void Init(ChargeMoveData move, GameObject owner)
        {
            _damageStats = move.damageStats;
            _hitEffects = move.hitEffects;
            _obstacleDestroyMask = move.obstacleDestroyMask;
            _owner = owner;
            
            _collider.radius = move.hitBoxRadius;
        }

        public void Activate() => _isActive = true;
        public void Deactivate() => _isActive = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive) return;
            
            var otherObject = other.gameObject;
            if (otherObject == _owner) return;
            
            // Obstacle layer -> destroy outright, no damage pipeline involved.
            if ((_obstacleDestroyMask & (1 << otherObject.layer)) != 0)
            {
                Destroy(otherObject);
                return;
            }
            
            // Anything else (player, other enemies) -> normal hit pipeline
            if (_hitEffects == null || _hitEffects.Length == 0) return;

            var hitContext = new HitContext
            {
                point = other.ClosestPoint(transform.position),
                direction = transform.forward,
                victim = otherObject,
                owner = _owner,
                stats = _damageStats
            };
            
            foreach (var effect in _hitEffects) effect.Apply(hitContext);
        }
    }
}