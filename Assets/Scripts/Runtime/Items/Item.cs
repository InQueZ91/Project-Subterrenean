using System;
using Data.Items;
using Data.Stats;
using Runtime.Handlers;
using UnityEngine;

namespace Runtime.Items
{
    [Serializable]
    public class Item
    {
        public ItemData Data { get; }
        public ItemStats Stats { get; set; }
        public int CurrentStacks { get; private set; }

        public Item(ItemData data, ItemStats resolvedStats, int stacks = 1)
        {
            Data = data;
            Stats = resolvedStats;
            CurrentStacks = stacks;
        }

        public bool Use(GameObject user)
        {
            if (Data is not UsableItemData usable) return false;
            if (CurrentStacks <= 0) return false;

            usable.Use(user);
            
            Consume();
            
            return true;
        }
        
        public void Consume()
        {
            if (CurrentStacks <= 0) return;
            CurrentStacks--;
        }

        public bool TryStack()
        {
            if (!Stats.isStackable) return false;
            if (CurrentStacks >= Stats.maxStack) return false;
            
            CurrentStacks++;
            return true;
        }
    }
}