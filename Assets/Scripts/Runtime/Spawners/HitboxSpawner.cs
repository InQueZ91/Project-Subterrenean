using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Spawners
{
    public class HitboxSpawner : MonoBehaviour
    {
        // Singleton
        public static HitboxSpawner Instance { get; private set; }

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
        
        private readonly Collider[] _hitBuffer = new Collider[32];

        public void Spawn(HitboxData data)
        {
            if (data.HitEffects == null || data.HitEffects.Length == 0) return;
            
            var normalizedDirection = data.Direction.normalized;
            var center = data.Origin + normalizedDirection * data.HitboxDistance;
            var hitCount = Physics.OverlapSphereNonAlloc(center, data.HitboxRadius, _hitBuffer, data.HitMask);

            for (var i = 0; i < hitCount; i++)
            {
                var victim = _hitBuffer[i].gameObject;

                var hitContext = new HitContext
                {
                    point = _hitBuffer[i].ClosestPoint(center),
                    direction = normalizedDirection,
                    victim = victim,
                    owner = data.Owner,
                    stats = data.DamageStats
                };
                
                foreach (var effect in data.HitEffects) effect.Apply(hitContext);
            }
        }
    }

    public struct HitboxData
    {
        public LayerMask HitMask { get; private set; }
        public DamageStats DamageStats { get; private set; }
        public HitEffect[] HitEffects { get; private set; }
        public float HitboxRadius { get; private set; }
        public float HitboxDistance { get; private set; }
        public Vector3 Origin { get; private set; }
        public Vector3 Direction { get; private set; }
        public GameObject Owner { get; private set; }

        public HitboxData(
            LayerMask hitMask,
            DamageStats damageStats,
            HitEffect[] hitEffects,
            float hitboxRadius,
            float hitboxDistance,
            Vector3 origin,
            Vector3 direction,
            GameObject owner)
        {
            HitMask = hitMask;
            DamageStats = damageStats;
            HitEffects = hitEffects;
            HitboxRadius = hitboxRadius;
            HitboxDistance = hitboxDistance;
            Origin = origin;
            Direction = direction;
            Owner = owner;
        }
    }
}