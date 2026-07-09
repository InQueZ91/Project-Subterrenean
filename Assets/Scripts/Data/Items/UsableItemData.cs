using UnityEngine;

namespace Data.Items
{
    public abstract class UsableItemData : ItemData
    {
        public abstract void Use(GameObject user);
    }
}