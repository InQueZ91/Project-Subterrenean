using Data.Items.Crafting;
using Runtime.Handlers;
using UnityEngine;

namespace Runtime
{
    public class PickupDetector : MonoBehaviour
    {
        private FabricatorHandler _fabricatorHandler;

        private void Awake()
        {
            _fabricatorHandler = GetComponent<FabricatorHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_fabricatorHandler == null) return;
            
            var loot = other.GetComponent<Loot>();
            if (loot == null || !loot.IsAvailable()) return;
            
            var pack = new ItemPack(loot.ItemData, loot.Quantity);
            var remaining = _fabricatorHandler.AddToStorage(pack);
            loot.OnCollectResult(remaining);
        }
    }
}