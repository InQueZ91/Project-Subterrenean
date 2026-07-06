using Runtime.Weapons.Supplies;
using UnityEngine;

namespace Data.Supplies
{
    /// <summary>
    /// Explicit marker - intentionally empty.
    /// This weapon can always fire. (e.g. melee weapons)
    /// </summary>
    [CreateAssetMenu(fileName = "Unlimited Supply", menuName = "Game/Supplies/Unlimited Supply")]
    public class UnlimitedSupplyData : SupplyData
    {
        public override ISupply CreateSupplyState() => new UnlimitedSupply();
    }
}