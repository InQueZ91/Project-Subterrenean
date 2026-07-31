using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Machine
{
    public class StorageSlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stacksText;
        
        private int _index;
        
        public Action<int> onAddItemToInventory; // Index

        public void Init(int index)
        {
            _index = index;
        }
        
        public void SetEmpty()
        {
            icon.enabled = false;
            stacksText.text = "";
        }

        public void SetItem(Sprite itemIcon, int stacks, int maxStack, bool isUsable)
        {
            icon.enabled = true;
            icon.sprite = itemIcon;
            stacksText.text = stacks > 0 ? $"{stacks}/{maxStack}" : "";
            icon.color = isUsable ? Color.greenYellow : Color.gray;
        }
        
        public void OnClicked()
        {
            onAddItemToInventory?.Invoke(_index);
        }
    }
}