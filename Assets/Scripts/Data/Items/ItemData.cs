using UnityEngine;

namespace Data.Items
{
    public abstract class ItemData : ScriptableObject
    {
        [Header("Stats")]
        public bool isStackable = true;
        public int maxStack = 10;
        
        [Header("Visuals")]
        public GameObject lootPrefab;
        public Sprite icon;
    }
}