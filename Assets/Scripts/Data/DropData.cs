using System;
using Data.Items;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class DropData
    {
        public ItemData itemData;
        [Min(1)]public int quantity;
        [Range(0f, 1f)] public float chance = 0.25f;
    }
}