using System.Collections.Generic;
using System.Linq;
using Data.Items;
using Data.Modules;
using Runtime.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Fabricator
{
    public class ConverterInfoUI : MonoBehaviour
    {
        [SerializeField] private GameObject recipePrefab;
        [SerializeField] private Transform recipeParent;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private ItemUI itemUI;

        public UnityEvent<int> onRecipeSelected; // recipeIndex
        private readonly List<ConversionRecipeUI> _recipes = new();

        private void Start()
        {
            foreach (Transform child in recipeParent)
                Destroy(child.gameObject);

            ClearModuleInfo();
        }

        public void BuildModuleInfo(Module module)
        {
            ClearModuleInfo();
            
            if (module == null) return;

            descriptionText.text = $"Module Name: {module.SourceData.name}\n" +
                                   $"Process Duration: {module.ProcessingDuration} sec\n" +
                                   $"State: {module.State}";

            var item = module.Container.FirstOrDefault();
            if (item == null) itemUI.SetEmpty();
            else
                itemUI.SetItem(
                    item.Data.icon,
                    item.CurrentStacks,
                    item.Data.maxStack,
                    item.Data is UsableItemData
                );

            if (module is not Converter converter) return;

            var recipes = converter.Recipes;
            var selected = converter.CurrentRecipe;
            if (recipes == null) return;
            if (recipes.Count == 0) return;

            BuildConversionRecipes(recipes.ToList(), selected);
        }

        private void ClearModuleInfo()
        {
            descriptionText.text = "";
            
            itemUI.SetEmpty();

            _recipes.Clear();

            foreach (Transform child in recipeParent)
                Destroy(child.gameObject);
        }

        private void BuildConversionRecipes(List<ConversionRecipe> recipes, ConversionRecipe selected)
        {
            if (_recipes.Count != recipes.Count)
            {
                _recipes.Clear();

                foreach (Transform child in recipeParent)
                    Destroy(child.gameObject);

                for (var i = 0; i < recipes.Count; i++)
                {
                    var go = Instantiate(recipePrefab, recipeParent);
                    var ui = go.GetComponent<ConversionRecipeUI>();
                    if (ui == null) return;

                    var recipeIndex = i;
                    ui.OnClicked += () => onRecipeSelected?.Invoke(recipeIndex);

                    _recipes.Add(ui);
                }
            }

            for (var i = 0; i < recipes.Count; i++)
            {
                var recipe = recipes[i];
                _recipes[i].SetRecipe(recipe, recipe == selected);
            }
        }
    }
}