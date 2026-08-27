using System;
using Data.Items;
using Data.Items.Crafting;
using UnityEngine;

namespace Runtime.Items
{
    [Serializable]
    public class Item
    {
        public ItemData Data { get; }
        public int CurrentStacks { get; private set; }

        public Item(ItemData data, int stacks = 1)
        {
            Data = data;
            CurrentStacks = stacks;
        }

        public bool Use(GameObject user)
        {
            if (Data is not UsableItemData usable) return false;
            if (TryConsume()) return false;
            
            usable.Use(user);
            
            return true;
        }
        
        public bool TryConsume()
        {
            if (CurrentStacks - 1 < 0) return false;
            CurrentStacks--;
            return true;
        }

        public bool TryStack()
        {
            if (CurrentStacks >= Data.maxStack) return false;
            
            CurrentStacks++;
            return true;
        }

        public ItemPack ToPack()
        {
            return new ItemPack { item = Data, quantity = CurrentStacks };
        }
    }
}