using System.Collections.Generic;
using Data.Items;
using Runtime.Items;

namespace Runtime.Handlers
{
    public interface IItemContainer
    {
        int TryAddItem(ItemData data, int quantity);
        bool TryRemoveItem(Item item);
        int Capacity { get; }
        IReadOnlyList<Item> Items { get; }
    }
}