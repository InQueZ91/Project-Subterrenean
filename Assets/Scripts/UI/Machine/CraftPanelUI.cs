using System.Collections.Generic;
using Data.Items.Crafting;
using Runtime.Handlers.Machine;
using UnityEngine;

namespace UI.Machine
{
    public class CraftPanelUI : MonoBehaviour
    {
        [SerializeField] private CraftHandler craftHandler;
        [SerializeField] private GameObject recipePrefab;
        
        private void Start()
        {
            if (craftHandler == null)
            {
                Debug.LogError("CraftPanelUI needs a CraftHandler");
            }

            ResetList();
            
            craftHandler.onRecipeListUpdated += RebuildList;
        }

        private void ResetList()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        public void RebuildList(List<ItemRecipe> recipes)
        {
            ResetList();
            
            foreach (var recipe in recipes)
            {
                var slot = Instantiate(recipePrefab, transform);
                var slotUI = slot.GetComponent<RecipeUI>();
                if (slotUI == null)
                {
                    Debug.LogError("Can't find RecipeUI slot in prefab");
                    return;
                }

                slotUI.SetRecipe(recipe);
                slotUI.onRecipeSelected += craftHandler.TryCraft;
            }
        }
    }
}