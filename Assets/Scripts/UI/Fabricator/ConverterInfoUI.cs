using System.Collections.Generic;
using System.Linq;
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

        public UnityEvent<int> onRecipeSelected; // recipeIndex
        private readonly List<ConversionRecipeUI> _recipes = new();
        
        private void Start()
        {
            foreach (Transform child in recipeParent)
                Destroy(child.gameObject);
        }

        public void BuildModuleInfo(Module module)
        {
            if (module == null)
            {
                ClearModuleInfo();
                return;
            }
            
            descriptionText.text = $"Module Name: {module.SourceData.name}\n" +
                                   $"Process Duration: {module.ProcessingDuration} sec" +
                                   $"State: {module.State}";

            if (module is not Converter converter) return;
            
            var recipes = converter.Recipes;
            if (recipes == null) return;
            if (recipes.Count == 0) return;
            
            BuildConversionRecipes(recipes.ToList());
        }

        private void ClearModuleInfo()
        {
            descriptionText.text = "";
            
            _recipes.Clear();
                
            foreach (Transform child in recipeParent)
                Destroy(child.gameObject);
        }

        private void BuildConversionRecipes(List<ConversionRecipe> recipes)
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
                _recipes[i].SetRecipe(recipe);
            }
        }
    }
}