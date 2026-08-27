using System;

namespace Data.Items.Crafting
{
    [Serializable]
    public class ItemPack
    {
        public ItemData item;
        public int quantity;

        public ItemPack()
        {
            
        }
        
        public ItemPack(ItemData item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }
}