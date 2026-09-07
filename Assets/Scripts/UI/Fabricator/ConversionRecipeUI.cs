using System;
using Data.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Fabricator
{
    public class ConversionRecipeUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image inputItem;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Image outputItem;
        [SerializeField] private TextMeshProUGUI outputQuantityText;

        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;

        public event Action OnClicked; 
        
        public void SetRecipe(ConversionRecipe recipe, bool isSelected)
        {
            if (recipe == null)
            {
                SetEmpty();
                return;
            }
            
            background.color = isSelected ? selectedColor : unselectedColor;
            inputItem.sprite = recipe.input.item.icon;
            quantityText.text = recipe.input.quantity.ToString();
            outputItem.sprite = recipe.output.item.icon;
            outputQuantityText.text = recipe.output.quantity.ToString();
        }

        private void SetEmpty()
        {
            background.color = unselectedColor;
            inputItem.sprite = null;
            quantityText.text = "";
            outputItem.sprite = null;
            outputQuantityText.text = "";
        }

        public void SelectRecipe()
        {
            OnClicked?.Invoke();
        }
    }
}