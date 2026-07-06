using System.Collections.Generic;
using Runtime;
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
        
        private void Awake()
        {
            _layout = GetComponent<HorizontalLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public void RebuildSlots(List<Item> items, int inventoryCapacity)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            ResizeLayout(inventoryCapacity);
            PopulateSlots(items, inventoryCapacity);
        }

        private void ResizeLayout(int inventoryCapacity)
        {
            var totalSlotsWidth = slotSize * inventoryCapacity;
            var totalSpacingWidth = _layout.spacing * (inventoryCapacity - 1);
            var layoutWidth = totalSlotsWidth + totalSpacingWidth;
            _rectTransform.sizeDelta = new Vector2(layoutWidth, slotSize);
        }

        private void PopulateSlots(List<Item> items, int inventoryCapacity)
        {
            for (var i = 0; i < inventoryCapacity; i++)
            {
                var item = i < items.Count ? items[i] : null;
                CreateSlot(item);
            }
        }

        private void CreateSlot(Item item)
        {
            var slot = Instantiate(slotPrefab, transform);
            var ui = slot.GetComponent<InventorySlotUI>();
            if (item is null)
            {
                ui.SetEmpty();
                return;
            }
            
            ui.SetItem(item.ItemData.icon, item.CurrentStacks);
        }
    }
}
