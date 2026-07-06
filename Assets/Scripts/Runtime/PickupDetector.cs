using Runtime.Handlers;
using UnityEngine;

namespace Runtime
{
    public class PickupDetector : MonoBehaviour
    {
        private InventoryHandler _inventory;

        private void Awake()
        {
            _inventory = GetComponentInParent<InventoryHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var loot = other.GetComponent<Loot>();
            if (loot == null) return;
            
            var remaining = _inventory.TryAddItem(loot.ItemData, loot.Quantity);
            loot.OnCollectResult(remaining);
        }
    }
}