using System.Collections.Generic;
using Data.Items.Crafting;
using Data.Modules;

namespace Runtime.Modules
{
    public class Converter : Module<ConverterData>
    {
        public IReadOnlyList<ConversionRecipe> Recipes => _recipes;
        private readonly ConversionRecipe[] _recipes;
        
        // Runtime
        public ConversionRecipe CurrentRecipe => _currentRecipe;
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
        
        protected override bool TryBegin(ItemBuffer upstream)
        {
            if (_currentRecipe is null) return false;
            if (upstream == null) return false;
            if (upstream.AvailableCount(Input.item) < Input.quantity) return false;
            
            upstream.ForcePull(Input);
            return true; // Success
        }

        protected override bool TryComplete()
        {
            return TryAdd(Output);
        }
    }
}