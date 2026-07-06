using Data.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime
{
    [RequireComponent(typeof(Collider))]
    public class Loot : MonoBehaviour
    {
        public ItemData ItemData {get; private set;}
        public int Quantity {get; private set;}
        private bool _pickedUp;
        
        public UnityEvent<ItemData, int> onCollected; // quantity

        public void Init(ItemData itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;
        }

        public void Collect()
        {
            if (ItemData == null || _pickedUp) return;
            
            _pickedUp = true;
            GetComponent<Collider>().enabled = false;
        }
        
        public void OnCollectResult(int remaining)
        {
            if (remaining <= 0)
            {
                onCollected?.Invoke(ItemData, Quantity);
                return;
            }

            // Inventory was full, reset state and remain in world
            Quantity = remaining;
            _pickedUp = false;
            GetComponent<Collider>().enabled = true;
        }
    }
}