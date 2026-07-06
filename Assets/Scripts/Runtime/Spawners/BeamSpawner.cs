using System.Collections.Generic;
using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Beam;
using UnityEngine;
using Visual;

namespace Runtime.Spawners
{
    public class BeamSpawner : MonoBehaviour
    {
        // Configuration
        [SerializeField] private LayerMask hitMask; // What beam ray can hit
        [SerializeField] private BeamRenderer beamRendererPrefab;
        [SerializeField] private int prewarmCount = 4;
        
        // Singleton
        public static BeamSpawner Instance { get; private set; }
        private readonly Queue<BeamRenderer> _pool = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            for (var i = 0; i < prewarmCount; i++)
                _pool.Enqueue(CreateRenderer());
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // Fire
        public void Spawn(
            HitEffect[] hitEffects,
            BeamStats stats,
            Vector3 origin,
            Vector3 direction,
            GameObject owner)
        {
            var segments = BeamSolver.Solve(origin, direction, stats.range, stats.maxBounces, hitMask);
            ApplyHitEffects(hitEffects, stats, segments, owner);
 
            var renderer = GetFromPool();
            renderer.Play(segments, ReturnToPool);
        }
        
        private static void ApplyHitEffects(
            HitEffect[] hitEffects,
            BeamStats stats,
            List<BeamSegment> segments,
            GameObject owner)
        {
            if (hitEffects == null || hitEffects.Length == 0) return;
 
            var damageStats = stats.ToDamageStats();
 
            foreach (var segment in segments)
            {
                if (segment.hitCollider == null) continue; // unobstructed final segment
 
                var hitContext = new HitContext
                {
                    point = segment.end,
                    direction = segment.direction,
                    victim = segment.hitCollider.gameObject,
                    owner = owner,
                    stats = damageStats,
                };
 
                foreach (var effect in hitEffects) effect.Apply(hitContext);
            }
        }
        
        // Pool
        private BeamRenderer CreateRenderer()
        {
            var renderer = Instantiate(beamRendererPrefab, transform);
            renderer.gameObject.SetActive(false);
            return renderer;
        }
 
        private BeamRenderer GetFromPool()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : CreateRenderer();
        }
 
        private void ReturnToPool(BeamRenderer renderer)
        {
            _pool.Enqueue(renderer);
        }
    }
}