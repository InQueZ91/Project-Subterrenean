using UnityEngine;

namespace Runtime.Spawners
{
    public class GeneralSpawner : MonoBehaviour
    {
        // Singleton
        public static GeneralSpawner Instance { get; private set; }

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
        
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;

            var go = Instantiate(prefab, position, rotation);
            go.transform.SetParent(transform);
            return go;
        }
    }
}