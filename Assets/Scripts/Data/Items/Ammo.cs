using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Ammo", menuName = "Game/Items/Ammo")]
    public class Ammo : ItemData
    {
        [Header("Specifics")]
        public AmmoType ammoType;
    }
}