using UnityEngine;

namespace Data.Items.Crafting
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Game/Items/New Recipe", order = 0)]
    public class ItemRecipe : ScriptableObject
    {
        public ItemPack inputA;
        public ItemPack inputB;
        public ItemPack output;
    }
}