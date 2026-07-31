using System;
using System.Collections.Generic;
using Data.Items.Crafting;
using UnityEngine;

namespace Runtime.Handlers.Machine
{
    [RequireComponent(typeof(StorageHandler))]
    public class CraftHandler : MonoBehaviour
    {
        [SerializeField] private List<ItemRecipe> itemRecipes = new();
        private List<ItemRecipe> ItemRecipes => itemRecipes;
        
        // Reference
        private StorageHandler _storageHandler;
        
        public Action<List<ItemRecipe>> onRecipeListUpdated;

        private void Awake()
        {
            _storageHandler = GetComponent<StorageHandler>();
        }

        private void Start()
        {
            onRecipeListUpdated?.Invoke(ItemRecipes);
        }

        private void OnValidate()
        {
            onRecipeListUpdated?.Invoke(ItemRecipes);
        }
        
        public bool TryCraft(ItemRecipe recipe)
        {
            if (recipe is null)
            {
                Debug.LogError($"Recipe is null");
                return false;
            }
            
            if (!_storageHandler.HasItem(recipe.inputA.item, recipe.inputA.quantity)) return false;
            if (!_storageHandler.HasItem(recipe.inputB.item, recipe.inputB.quantity)) return false;
            if (!_storageHandler.CanFit(recipe.output.item, recipe.output.quantity)) return false;

            _storageHandler.TryConsumeItem(recipe.inputA.item, recipe.inputA.quantity);
            _storageHandler.TryConsumeItem(recipe.inputB.item, recipe.inputB.quantity);
            _storageHandler.TryAddItem(recipe.output.item, recipe.output.quantity);
            
            return true;
        }
    }
}