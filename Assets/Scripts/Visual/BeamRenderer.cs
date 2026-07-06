using System;
using System.Collections.Generic;
using Runtime.Beam;
using UnityEngine;

namespace Visual
{
    /// <summary>
    /// Pure visual - draws a solved beam path and play impact VFX at each
    /// bounce/hit point. Knows nothing about damage, hit effects, or gameplay
    /// Pooled and returned via callback after its lifetime expires.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class BeamRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float lifetime = 0.15f;
        [SerializeField] private GameObject impactVfxPrefab;
        
        private readonly List<GameObject> _spawnedImpacts = new();
        private Action<BeamRenderer> _returnToPool;
        private float _despawnTime;
        private bool _active;

        private void Awake()
        {
            if (lineRenderer == null) 
                lineRenderer = GetComponent<LineRenderer>();
        }

        public void Play(List<BeamSegment> segments, Action<BeamRenderer> returnToPool)
        {
            if (segments == null || segments.Count == 0) return;

            _returnToPool = returnToPool;
            DrawPath(segments);
            SpawnImpacts(segments);
            
            gameObject.SetActive(true);
            _despawnTime = Time.time + lifetime;
            _active = true;
        }

        private void DrawPath(List<BeamSegment> segments)
        {
            var points = new Vector3[segments.Count + 1];
            points[0] = segments[0].start;
            for (var i = 0; i < segments.Count; i++)
                points[i + 1] = segments[i].end;
            
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        private void SpawnImpacts(List<BeamSegment> segments)
        {
            if (impactVfxPrefab == null) return;

            foreach (var segment in segments)
            {
                if (segment.hitCollider == null) continue; // final unobstructed segment, nothing to impact
                
                var rotation = segment.hitNormal.HasValue
                    ? Quaternion.LookRotation(segment.hitNormal.Value)
                    : Quaternion.identity;
                
                var vfx = Instantiate(impactVfxPrefab, segment.end, rotation);
                _spawnedImpacts.Add(vfx);
            }
        }

        private void Update()
        {
            if (!_active) return;
            if (Time.time < _despawnTime) return;

            Cleanup();
        }

        private void Cleanup()
        {
            _active = false;

            foreach (var impact in _spawnedImpacts)
                if (impact != null) Destroy(impact);

            _spawnedImpacts.Clear();
            
            gameObject.SetActive(false);
            _returnToPool?.Invoke(this);
        }
    }
}