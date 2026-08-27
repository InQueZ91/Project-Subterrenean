using System;
using System.Collections.Generic;
using System.Linq;
using Data.Items;
using Data.Items.Crafting;
using Runtime.Items;
using UnityEngine;

namespace Runtime
{
    public abstract class ItemBuffer : IItemBufferReader
    {
        public int Capacity { get; private set; }
        public event Action OnChanged;
        
        public IReadOnlyList<Item> Container => container;
        private readonly List<Item> container;

        protected ItemBuffer(int capacity)
        {
            Capacity = capacity;
            container = new List<Item>();
        }

        // API
        public int AvailableCount(ItemData itemData)
        {
            return container
                .Where(i => i.Data == itemData)
                .Sum(i => i.CurrentStacks);
        }

        public bool CanAdd(ItemPack pack)
        {
            var existingSpace = container
                .Where(i => i.Data == pack.item)
                .Sum(i => i.Data.maxStack - i.CurrentStacks);

            var newSlotsNeeded = Mathf.CeilToInt(
                Mathf.Max(0, pack.quantity - existingSpace) / (float)pack.item.maxStack
            );

            return container.Count + newSlotsNeeded <= Capacity;
        }

        public bool TryAdd(ItemPack pack)
        {
            if (!CanAdd(pack)) return false;

            Add(pack);
            return true;
        }

        public int ForceAdd(ItemPack pack)
        {
            return Add(pack);
        }

        public bool TryPull(ItemPack pack)
        {
            if (!HasItem(pack)) return false;

            Pull(pack);
            return true;
        }

        public int ForcePull(ItemPack pack)
        {
            return Pull(pack);
        }

        public List<ItemPack> PullAll()
        {
            var pulled = new List<Item>(container);
            container.Clear();
            NotifyChanged();
            return pulled.Select(item => item.ToPack()).ToList();
        }

        public void RemoveAt(int index)
        {
            container.RemoveAt(index);
            NotifyChanged();
        }

        // Helpers
        private void NotifyChanged()
        {
            OnChanged?.Invoke();
        }

        private bool HasItem(ItemPack pack)
        {
            return container
                .Where(i => i.Data == pack.item)
                .Sum(i => i.CurrentStacks) >= pack.quantity;
        }

        private int Add(ItemPack pack)
        {
            var quantity = pack.quantity;
            if (quantity < 0) return 0;

            // Fill existing stacks first
            foreach (var item in container.Where(i => i.Data == pack.item).ToList())
            {
                while (quantity > 0 && item.TryStack())
                    quantity--;
            }

            // Add new slots for remainder
            while (container.Count < Capacity && quantity > 0)
            {
                var stackSize = Math.Min(quantity, pack.item.maxStack);
                container.Add(new Item(pack.item, stackSize));
                quantity -= stackSize;
            }

            NotifyChanged();
            
            return quantity;
        }

        private int Pull(ItemPack pack)
        {
            var requested = pack.quantity;

            // Consume from fewest stack first
            var ordered = container
                .Where(i => i.Data == pack.item)
                .OrderBy(i => i.CurrentStacks).ToList();

            foreach (var item in ordered)
            {
                while (requested > 0 && item.TryConsume())
                    requested--;
            }

            // Remove empty stacks
            container.RemoveAll(i => i.CurrentStacks == 0);

            NotifyChanged();
            
            return pack.quantity - requested;
        }
    }

    public interface IItemBufferReader
    {
        int Capacity { get; }
        IReadOnlyList<Item> Container { get; }
        event Action OnChanged;
    }
}