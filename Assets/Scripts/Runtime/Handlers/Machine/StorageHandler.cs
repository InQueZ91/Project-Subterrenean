using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Items;
using Runtime.Items;
using Runtime.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Handlers.Machine
{
    public class StorageHandler : MonoBehaviour, IItemContainer
    {
        [SerializeField] private InventoryHandler playerInventoryHandler;
        [SerializeField] private int storageCapacity = 21;
        
        public int Capacity => storageCapacity;
        public IReadOnlyList<Item> Items => _storage;
        private readonly List<Item> _storage = new();
        
        [Header("Event")]
        public UnityEvent<List<Item>, int> onStorageChanged; // inventory, capacity
        
        private void NotifyInventoryChanged()
        {
            onStorageChanged?.Invoke(_storage, storageCapacity);
        }

        public void Init(StartingItemData[] startingItems)
        {
            _storage.Clear();
            foreach (var item in startingItems)
                TryAddItem(item.itemData, item.quantity);
        }

        public void AddToInventory(int index)
        {
            if (index >= _storage.Count) return;
            
            var itemToPull = _storage[index];
            
            playerInventoryHandler?.TryAddItem(itemToPull.Data, itemToPull.CurrentStacks);
            
            _storage.RemoveAt(index);
            
            Compact();
            NotifyInventoryChanged();
        }

        public bool CanFit(ItemData itemData, int quantity)
        {
            var existingSpace = _storage
                .Where(i => i.Data == itemData)
                .Sum(i => i.Data.maxStack - i.CurrentStacks);

            var newSlotsNeeded = Mathf.CeilToInt(
                Mathf.Max(0, quantity - existingSpace) / (float)itemData.maxStack
            );

            return (_storage.Count + newSlotsNeeded) <= storageCapacity;
        }
        
        public bool HasItem(ItemData itemData, int quantity)
        {
            return _storage
                .Where(i => i.Data == itemData)
                .Sum(i => i.CurrentStacks) >= quantity;
        }

        public bool TryConsumeItem(ItemData itemData, int quantity)
        {
            var totalAvailable = _storage
                .Where(i => i.Data == itemData)
                .Sum(i => i.CurrentStacks);

            if (totalAvailable < quantity) return false;
            
            var remaining = quantity;
            foreach (var stack in _storage.Where(i => i.Data == itemData).ToList())
            {
                while (remaining > 0 && stack.CurrentStacks > 0)
                {
                    stack.Consume();
                    remaining--;
                }

                if (remaining <= 0) break;
            }
            
            Compact();
            NotifyInventoryChanged();
            return true;
        }
        
        public int TryAddItem(ItemData itemData, int quantity)
        {
            if (quantity <= 0) return 0;

            // Fill existing stacks first
            foreach (var stack in _storage.Where(i => i.Data == itemData).ToList())
            {
                while (quantity > 0 && stack.TryStack())
                    quantity--;
            }

            // Add new slots for remainder
            while (_storage.Count < storageCapacity && quantity > 0)
            {
                var stackSize = Math.Min(quantity, itemData.maxStack);
                _storage.Add(new Item(itemData, stackSize));
                quantity -= stackSize;
            }

            NotifyInventoryChanged();

            return quantity;
        }

        public bool TryRemoveItem(Item item)
        {
            throw new NotImplementedException();
        }

        private void Compact()
        {
            if (_storage.Count == 0) return;

            var compacted = new List<Item>(_storage.Count);

            foreach (var itemGroup in _storage.GroupBy(i => i.Data))
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

            _storage.Clear();
            _storage.AddRange(compacted);
        }
        
    }
}