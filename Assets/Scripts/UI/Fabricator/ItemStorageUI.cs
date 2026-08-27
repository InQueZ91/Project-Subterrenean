using System.Collections.Generic;
using Data.Items;
using Runtime.Items;
using UnityEngine;

namespace UI.Fabricator
{
    public class ItemStorageUI : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform storageSlotParent;

        private readonly List<GameObject> _slots = new();

        public void RebuildSlots(List<Item> items, int capacity)
        {
            // Only rebuild structure if capacity changed
            // if (_slots.Count != capacity)
            // {
                foreach (Transform child in storageSlotParent)
                    Destroy(child.gameObject);

                _slots.Clear();

                for (var i = 0; i < capacity; i++)
                    _slots.Add(Instantiate(slotPrefab, storageSlotParent));
            // }

            // Always update slot data
            for (var i = 0; i < _slots.Count; i++)
            {
                var item = i < items.Count ? items[i] : null;
                if (item == null) continue;
                
                var go = Instantiate(itemPrefab, _slots[i].transform);
                var itemUI = go.GetComponent<ItemUI>();
                if (itemUI == null) continue;
                
                itemUI.Init(i);
                itemUI.SetItem(
                    item.Data.icon,
                    item.CurrentStacks,
                    item.Data.maxStack,
                    item.Data is UsableItemData);
            }
        }
    }
}