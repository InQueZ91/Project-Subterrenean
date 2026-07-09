using Data;
using Data.Items;
using UnityEngine;

namespace Runtime.Spawners
{
    public class LootSpawner : MonoBehaviour
    {
        // Configuration
        [SerializeField] private LayerMask lootLayer; // layer assigned to loot
        [SerializeField] private float collectDelay = 0.5f;

        // Singleton
        public static LootSpawner Instance { get; private set; }

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
        
        public void Spawn(ItemData itemData, int quantity, Vector3 position)
        {
            if (itemData.lootPrefab == null) return;

            var go = Instantiate(itemData.lootPrefab, position, Quaternion.identity);
            var loot = go.GetComponent<Loot>();

            if (loot == null)
            {
                Debug.LogWarning($"[LootSpawner] Prefab for '{itemData.name}' is missing a Loot component.");
                Destroy(go);
                return;
            }

            go.transform.SetParent(transform);
            go.layer = Mathf.RoundToInt(Mathf.Log(lootLayer.value, 2));
            loot.Init(itemData, quantity, collectDelay);
        }

        public void SpawnFromDropTable(DropData[] dropTable, Vector3 position)
        {
            foreach (var drop in dropTable)
            {
                if (drop.itemData == null) continue;

                if (Random.value <= drop.chance)
                {
                    Spawn(drop.itemData, drop.quantity, position);
                    break;
                }
            }
        }
    }
}