using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stacksText;

        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
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
        
        public void Select()
        {
            _canvasGroup.alpha = 1;
        }

        public void Deselect()
        {
            _canvasGroup.alpha = 0.5f;
        }
    }
}
