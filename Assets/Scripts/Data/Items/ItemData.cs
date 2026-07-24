using Data.Stats;
using UnityEngine;

namespace Data.Items
{
    public abstract class ItemData : ScriptableObject
    {
        public ItemStats itemStats;
        
        [Header("Visuals")]
        public GameObject lootPrefab;
        public Sprite icon;
    }
}