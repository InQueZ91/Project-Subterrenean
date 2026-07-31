using System;
using Data.Items;
using Data.Items.Crafting;
using UnityEngine;

namespace UI.Machine
{
    public class RecipeUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI inputA;
        [SerializeField] private InventorySlotUI inputB;
        [SerializeField] private InventorySlotUI output;

        private ItemRecipe _recipe;
        
        public Func<ItemRecipe, bool> onRecipeSelected;
        
        public void SetRecipe(ItemRecipe recipe)
        {
            _recipe = recipe;
            
            SetSlot(inputA, recipe.inputA);
            SetSlot(inputB, recipe.inputB);
            SetSlot(output, recipe.output);
        }

        private void SetSlot(InventorySlotUI slot, ItemPack itemPack)
        {
            slot.SetItem(
                itemPack.item.icon,
                itemPack.quantity,
                itemPack.item.maxStack,
                itemPack.item is UsableItemData
            );
            
            slot.Select();
        }

        public void OnClicked()
        {
            if (onRecipeSelected?.Invoke(_recipe) ?? false)
            {
                // On Craft Success
            }
        }
    }
}