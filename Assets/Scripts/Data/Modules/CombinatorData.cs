using System.Collections.Generic;
using Runtime.Modules;
using UnityEngine;

namespace Data.Modules
{
    [CreateAssetMenu(fileName = "New Combinator Data", menuName = "Game/Modules/Combinator")]
    public class CombinatorData : ModuleData<Combinator>
    {
        public List<CombinationRecipe> combinationRecipes;
        protected override Combinator Build() => new Combinator(this);
    }
}