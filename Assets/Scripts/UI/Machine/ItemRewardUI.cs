using Data;
using Data.Items.Crafting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Machine
{
    public class ItemRewardUI : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image fill;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private TextMeshProUGUI costText;
        
        [SerializeField] private Sprite placeholderIcon;

        private void Start()
        {
            quantityText.text = "";
        }

        public void SetReward(ItemPack reward)
        {
            if (reward == null)
            {
                HideReward();
                itemIcon.sprite = placeholderIcon;
                quantityText.text = "";
                return;
            }
            
            ShowReward();
            itemIcon.sprite = reward.item.icon;
            quantityText.text = reward.quantity.ToString();
        }

        public void SetCost(int cost)
        {
            costText.text = $"Roll {cost}Cp";
            costText.color = Color.white;
        }
        
        public void Failed()
        {
            costText.color = Color.red;
        }

        private void ShowReward()
        {
            fill.color = new Color(0.5f, 0.5f, 0.5f, 0);
        }

        private void HideReward()
        {
            fill.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }
}