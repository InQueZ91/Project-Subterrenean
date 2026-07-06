using System;
using Data.Items;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class StartingItemData
    {
        public ItemData itemData;
        [Min(0)] public int quantity;
    }
}