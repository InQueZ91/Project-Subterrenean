using UnityEngine;

namespace Data.Items
{
    public abstract class ItemData : ScriptableObject
    {
        [Min(1)]
        public int maxStack;
        
        [Header("Visuals")]
        public GameObject lootPrefab;
        public Sprite icon;
    }
}