using Data.Items.Crafting;
using UnityEngine;

namespace Data.Modules
{
    [CreateAssetMenu(fileName = "New Conversion Recipe",menuName = "Game/Modules/Conversion Recipe")]
    public class ConversionRecipe : ScriptableObject
    {
        public ItemPack input;
        public ItemPack output;
        public float conversionTime;
    }
}