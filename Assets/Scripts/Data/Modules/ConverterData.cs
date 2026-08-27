using System.Collections.Generic;
using Runtime.Modules;
using UnityEngine;

namespace Data.Modules
{
    [CreateAssetMenu(fileName = "New Converter Data", menuName = "Game/Modules/New Converter")]
    public class ConverterData : ModuleData<Converter>
    {
        public List<ConversionRecipe> conversionRecipes;
        protected override Converter Build() => new Converter(this);
    }
}