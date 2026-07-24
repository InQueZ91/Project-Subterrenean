using System.Collections.Generic;
using Data.Items;
using Runtime.Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private int slotSize = 80;
        
        private HorizontalLayoutGroup _layout;
        private RectTransform _rectTransform;
        
        private readonly List<InventorySlotUI> _slots = new();
        
        private void Awake()
        {
            _layout = GetComponent<HorizontalLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SelectSlot(int index)
        {
            if (_slots.Count <= 0) return;
            _slots.ForEach(s => s.Deselect());
            _slots[index].Select();
        }
        
        public void RebuildSlots(List<Item> items, int inventoryCapacity)
        {
            // Only rebuild structure if capacity changed
            if (_slots.Count != inventoryCapacity)
            {
                foreach (Transform child in transform)
                    Destroy(child.gameObject);
                
                _slots.Clear();
                ResizeLayout(inventoryCapacity);
                for (var i = 0; i < inventoryCapacity; i++)
                {
                    var slot = Instantiate(slotPrefab, transform);
                    _slots.Add(slot.GetComponent<InventorySlotUI>());
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
                    item.Stats.maxStack,
                    item.Data is UsableItemData);
            }
        }
        
        private void ResizeLayout(int inventoryCapacity)
        {
            var totalSlotsWidth = slotSize * inventoryCapacity;
            var totalSpacingWidth = _layout.spacing * (inventoryCapacity - 1);
            var layoutWidth = totalSlotsWidth + totalSpacingWidth;
            _rectTransform.sizeDelta = new Vector2(layoutWidth, slotSize);
        }
    }
}
