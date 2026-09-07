using Data.Items.Crafting;
using UnityEngine;

namespace Data.Modules
{
    [CreateAssetMenu(fileName = "New Combination Recipe", menuName = "Game/Modules/Combination Recipe")]
    public class CombinationRecipe : ScriptableObject
    {
        public ItemPack inputA;
        public ItemPack inputB;
        public ItemPack output;
        public float combinationTime;
    }
}