using System.Collections.Generic;
using System.Linq;
using Data.Items.Crafting;
using Data.Modules;

namespace Runtime.Modules
{
    public class Converter : Module<ConverterData>
    {
        public IReadOnlyList<ConversionRecipe> Recipes => _recipes;
        private readonly ConversionRecipe[] _recipes;
        
        // Runtime
        private ConversionRecipe _currentRecipe;
        private ItemPack Input => _currentRecipe.input;
        private ItemPack Output => _currentRecipe.output;
        
        public Converter(ConverterData converterData)  : base(converterData)
        {
            _recipes = converterData.conversionRecipes.ToArray();
            TrySetRecipe(0);
        }

        public bool TrySetRecipe(int index)
        {
            if (index >= _recipes.Length) return false;
            
            if (State == ModuleState.Processing) return false;
            
            var recipe = _recipes[index];
            if (recipe is null) return false;
            
            _currentRecipe = recipe;
            ProcessingDuration = _currentRecipe.conversionTime;
            return true;
        }
        
        protected override bool TryBegin(List<ItemBuffer> connectedModules)
        {
            if (_currentRecipe is null) return false;
            
            var totalAvailable = connectedModules.Sum(m => m.AvailableCount(Input.item));
            if (totalAvailable < Input.quantity) return false; // Fail
            
            var required = Input.quantity;
            foreach (var module in connectedModules)
            {
                if (required <= 0) break;
                required -= module.ForcePull(new ItemPack(Input.item, required));
            }
            
            return true; // Success
        }

        protected override bool TryComplete()
        {
            return TryAdd(Output);
        }
    }
}