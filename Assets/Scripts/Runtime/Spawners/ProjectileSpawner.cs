using System.Collections.Generic;
using Data.Output;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Spawners
{
    public class ProjectileSpawner : MonoBehaviour
    {
        // Configuration
        [SerializeField] private LayerMask projectileLayer; // Layer assigned to projectiles
        [SerializeField] private LayerMask hitMask; // What projectile raycasts can hit
    
        // Singleton
        public static ProjectileSpawner Instance { get; private set; }
    
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
    
        // Pool
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();

        // Spawn / Return
        public void Spawn(
            ProjectileOutputData outputData,
            ProjectileStats modifiedStats,
            Vector3 origin,
            Vector3 direction,
            GameObject owner)
        {
            if (outputData.prefab == null)
            {
                Debug.LogWarning("ProjectileSpawner: missing prefab.");
                return;
            }
        
            var go = GetFromPool(outputData.prefab);
            go.transform.SetPositionAndRotation(origin, Quaternion.LookRotation(direction));
            go.layer = Mathf.RoundToInt(Mathf.Log(projectileLayer.value, 2));
            go.SetActive(true);

            var p = go.GetComponent<Projectile>();
            p.Init(outputData.prefab, modifiedStats, outputData.hitEffects, direction, owner, hitMask);
        }

        public void Return(GameObject go, GameObject prefab)
        {
            go.SetActive(false);
            if (!_pools.ContainsKey(prefab)) 
                _pools.Add(prefab, new Queue<GameObject>());
            _pools[prefab].Enqueue(go);
        }

        // internal pool fetch
        private GameObject GetFromPool(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab, out var pool) && pool.Count > 0)
                return pool.Dequeue();
        
            // Pool empty -> create new one
            var go = Instantiate(prefab, transform, true);
            if (go.GetComponent<Projectile>() == null)
                go.AddComponent<Projectile>();
            return go;
        }

        // Clear all pools (call on scene load)
        public void ClearAll()
        {
            foreach (var pool in _pools.Values)
            foreach (var go in pool)
                if (go != null) Destroy(go);
            _pools.Clear();
        }
    }
}
