using System;
using Data.Items;

namespace Runtime
{
    [Serializable]
    public class Item
    {
        public ItemData ItemData { get; }
        
        public int CurrentStacks { get; private set; }

        public Item(ItemData itemData, int stacks = 1)
        {
            ItemData = itemData;
            CurrentStacks = stacks;
        }
        
        public void Consume()
        {
            if (CurrentStacks <= 0) return;
            CurrentStacks--;
        }

        public bool TryStack()
        {
            if (!ItemData.isStackable) return false;
            if (CurrentStacks >= ItemData.maxStack) return false;
            
            CurrentStacks++;
            return true;
        }
    }
}