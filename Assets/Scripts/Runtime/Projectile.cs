using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Spawners;
using UnityEngine;

namespace Runtime
{
    public class Projectile : MonoBehaviour
    {
        // References
        private GameObject _prefab;
        private GameObject _owner;
        private Rigidbody _rb;
        private HitEffect[] _hitEffects;
        private ProjectileStats _stats;
        private LayerMask _hitMask;
        
        // Runtime state
        private Vector3 _direction;
        private float _spawnTime;
        private int _remainingHits;
        private int _remainingBounces;
        private bool _hasHit;
        
        // Constants
        private const float RicochetNudge = 0.01f;
        private const float RicochetMinDot = -0.1f;
        
        // Init
        public void Init(
            GameObject prefab,
            ProjectileStats modifiedStats,
            HitEffect[] hitEffects,
            Vector3 direction,
            GameObject owner,
            LayerMask hitMask)
        {
            _prefab = prefab;
            _stats = modifiedStats;
            _hitEffects = hitEffects;
            _direction = direction.normalized;
            _owner = owner;
            _hitMask = hitMask;
            
            _spawnTime = Time.time;
            _remainingHits = _stats.pierce;
            _remainingBounces = _stats.ricochet;
            _hasHit = false;
        }

        // Unity lifecycle
        private void Update()
        {
            if (_hasHit) return;

            ApplyGravity();
            CheckRange();
            CheckRaycast();
            
            // Move after raycast so we never skip past a surface
            transform.position += _direction * (_stats.speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(_direction);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHit) return;
            OnHit(other, transform.position, null);
        }
        
        // Movement
        private void ApplyGravity()
        {
            if (!_stats.useGravity) return;
            
            // Accumulate downward velocity into direction each frame
            _direction = (_direction * _stats.speed + Physics.gravity * Time.deltaTime).normalized;
        }
        
        private void CheckRange()
        {
            var distanceTravelled = (Time.time - _spawnTime) * _stats.speed;
            if (distanceTravelled > _stats.range) Despawn();
        }
        
        private void CheckRaycast()
        {
            var moveDist = _stats.speed * Time.deltaTime;
            if (!Physics.Raycast(transform.position, _direction, out var hit, moveDist, _hitMask)) 
                return;
            
            OnHit(hit.collider, hit.point, hit.normal);
        }
        
        // Hit handling
        private void OnHit(Collider victim, Vector3 hitPoint, Vector3? surfaceNormal)
        {
            if (victim.gameObject == _owner) return;
            if (victim.GetComponent<Projectile>() != null) return;

            ApplyHitEffects(hitPoint, victim);
            
            if (TryRicochet(hitPoint, surfaceNormal)) return;
            if (TryPierce()) return;
            
            Despawn();
        }

        private void ApplyHitEffects(Vector3 hitPoint, Collider victim)
        {
            var hitContext = new HitContext
            {
                point = hitPoint,
                direction = _direction,
                victim = victim.gameObject,
                owner = _owner,
                stats = _stats.ToDamageStats(),
            };
            foreach (var hitEffect in _hitEffects) hitEffect.Apply(hitContext);
        }

        private bool TryRicochet(Vector3 hitPoint, Vector3? surfaceNormal)
        {
            if (_remainingBounces <= 0 || !surfaceNormal.HasValue) return false;
            
            var flatNormal = new Vector3(surfaceNormal.Value.x, 0f, surfaceNormal.Value.z).normalized;
 
            if (flatNormal == Vector3.zero) return false; // floor/ceiling — no ricochet
 
            var dot = Vector3.Dot(_direction, flatNormal);
            if (dot >= RicochetMinDot) return false; // too head-on — no ricochet
 
            _direction = Vector3.Reflect(_direction, flatNormal).normalized;
            transform.position = hitPoint + surfaceNormal.Value * RicochetNudge;
            _remainingBounces--;
            return true;
        }
        
        private bool TryPierce()
        {
            if (_remainingHits <= 0) return false;
 
            _remainingHits--;
            return true;
        }
        
        // Despawn
        private void Despawn()
        {
            _hasHit = true;
            ProjectileSpawner.Instance.Return(gameObject, _prefab);
        }
    }
}