using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Handlers
{
    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField] private int inventoryCapacity = 6;

        private readonly List<Item> _inventory = new();

        // Events
        public UnityEvent<List<Item>, int> onInventoryChanged; // inventory, capacity
        
        public void Init(StartingItemData[] startingItems)
        {
            _inventory.Clear();
            foreach (var item in startingItems)
                TryAddItem(item.itemData, item.quantity);

            onInventoryChanged?.Invoke(_inventory, inventoryCapacity);
        }

        public int GetAmmoCount(AmmoType ammoType)
        {
            return GetAmmo(ammoType)?.CurrentStacks ?? 0;
        }

        public int ConsumeAmmo(AmmoType ammoType, int quantity)
        {
            var ammoItem = GetAmmo(ammoType);
            if (ammoItem == null) return 0;

            var consumed = 0;
            while (consumed < quantity && ammoItem.CurrentStacks > 0)
            {
                ammoItem.Consume();
                consumed++;
            }

            // Clean up the slot if empty
            if (ammoItem.CurrentStacks <= 0)
                _inventory.Remove(ammoItem);

            CompactInventory();
            onInventoryChanged?.Invoke(_inventory, inventoryCapacity);
            return consumed;
        }

        private void CompactInventory()
        {
            if (_inventory.Count == 0) return;

            var compacted = new List<Item>(_inventory.Count);

            foreach (var itemGroup in _inventory.GroupBy(i => i.ItemData))
            {
                var maxStack = itemGroup.Key.maxStack;
                if (maxStack <= 0) continue;

                var remaining = itemGroup.Sum(i => i.CurrentStacks);

                while (remaining > 0)
                {
                    var stackSize = Math.Min(remaining, maxStack);
                    compacted.Add(new Item(itemGroup.Key, stackSize));
                    remaining -= stackSize;
                }
            }

            _inventory.Clear();
            _inventory.AddRange(compacted);
        }

        private Item GetAmmo(AmmoType ammoType)
        {
            return _inventory.Find(i => i.ItemData is Ammo a && a.ammoType == ammoType);
        }

        public int TryAddItem(ItemData itemData, int quantity)
        {
            if (quantity <= 0) return 0;

            // Fill existing stacks first
            foreach (var stack in _inventory.Where(i => i.ItemData == itemData).ToList())
            {
                while (quantity > 0 && stack.TryStack())
                    quantity--;
            }

            // Add new slots for remainder
            while (_inventory.Count < inventoryCapacity && quantity > 0)
            {
                var stackSize = Math.Min(quantity, itemData.maxStack);
                _inventory.Add(new Item(itemData, stackSize));
                quantity -= stackSize;
            }

            onInventoryChanged?.Invoke(_inventory, inventoryCapacity);

            return quantity;
        }
    }
}