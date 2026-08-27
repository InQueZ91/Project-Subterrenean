using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Fabricator
{
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stacksText;
        
        public int SlotIndex { get; private set; }

        public void Init(int index)
        {
            SlotIndex = index;
        }

        public void SetItem(Sprite itemIcon, int stacks, int maxStack, bool isUsable)
        {
            icon.enabled = true;
            icon.sprite = itemIcon;
            stacksText.text = stacks > 0 ? $"{stacks}/{maxStack}" : "";
            icon.color = isUsable ? Color.greenYellow : Color.gray;
        }
    }
}