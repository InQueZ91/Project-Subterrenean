using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Spawners
{
    public class MeleeSpawner : MonoBehaviour
    {
        // Configuration
        [SerializeField] private LayerMask hitMask;

        // Singleton
        public static MeleeSpawner Instance { get; private set; }

        // Non-alloc buffer
        private const int BufferSize = 32;
        private readonly Collider[] _hitBuffer = new Collider[BufferSize];

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // Spawn
        public void Spawn(
            HitEffect[] hitEffects,
            MeleeStats stats,
            Vector3 origin,
            Vector3 direction,
            GameObject owner)
        {
            var center = origin + direction.normalized * stats.range;
            var hitCount = Physics.OverlapSphereNonAlloc(center, stats.hitboxRadius, _hitBuffer, hitMask);

            if (hitEffects == null || hitEffects.Length == 0) return;

            var damageStats = stats.ToDamageStats();

            for (var i = 0; i < hitCount; i++)
            {
                var victim = _hitBuffer[i].gameObject;

                var hitContext = new HitContext
                {
                    point = _hitBuffer[i].ClosestPoint(origin),
                    direction = direction,
                    victim = victim,
                    owner = owner,
                    stats = damageStats,
                };

                foreach (var effect in hitEffects) effect.Apply(hitContext);
            }
        }
    }
}