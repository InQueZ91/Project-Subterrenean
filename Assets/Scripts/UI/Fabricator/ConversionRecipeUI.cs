using System;
using Data.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Fabricator
{
    public class ConversionRecipeUI : MonoBehaviour
    {
        [SerializeField] private Image inputItem;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Image outputItem;
        [SerializeField] private TextMeshProUGUI outputQuantityText;

        public event Action OnClicked; 
        
        public void SetRecipe(ConversionRecipe recipe)
        {
            if (recipe == null)
            {
                SetEmpty();
                return;
            }
            
            inputItem.sprite = recipe.input.item.icon;
            quantityText.text = recipe.input.quantity.ToString();
            outputItem.sprite = recipe.output.item.icon;
            outputQuantityText.text = recipe.output.quantity.ToString();
        }

        public void SetEmpty()
        {
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