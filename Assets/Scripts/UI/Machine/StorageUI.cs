using System.Collections.Generic;
using Data.Items;
using Runtime.Items;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Machine
{
    public class StorageUI : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private int slotSize = 100;
        
        private readonly List<StorageSlotUI> _slots = new();
        
        [Header("Event")]
        public UnityEvent<int> onAddItemToInventory; // item index
        
        public void RebuildSlots(List<Item> items, int inventoryCapacity)
        {
            // Only rebuild structure if capacity changed
            if (_slots.Count != inventoryCapacity)
            {
                foreach (Transform child in transform)
                    Destroy(child.gameObject);
                
                _slots.Clear();
                
                for (var i = 0; i < inventoryCapacity; i++)
                {
                    var slot = Instantiate(slotPrefab, transform);
                    
                    var slotUI = slot.GetComponent<StorageSlotUI>();
                    if (slotUI == null) continue;
                    slotUI.Init(i);
                    slotUI.onAddItemToInventory += OnAddItemToInventory;
                    
                    _slots.Add(slotUI);
                }
            }
            
            // Always update slot data
            for (var i = 0; i < _slots.Count; i++)
            {
                var item = i < items.Count ? items[i] : null;
                if (item == null) _slots[i].SetEmpty();
                else _slots[i].SetItem(
                    item.Data.icon,
                    item.CurrentStacks,
                    item.Data.maxStack,
                    item.Data is UsableItemData);
            }
        }

        private void OnAddItemToInventory(int index)
        {
            onAddItemToInventory?.Invoke(index);
        }
    }
}