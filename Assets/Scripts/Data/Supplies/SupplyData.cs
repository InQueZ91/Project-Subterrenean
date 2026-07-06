using Runtime.Weapons.Supplies;
using UnityEngine;

namespace Data.Supplies
{
    /// <summary>
    /// Base for "how a weapon is fueled." Optional on WeaponData - melee
    /// weapons can leave this unassigned or point at InfiniteAmmoData
    /// </summary>
    public abstract class SupplyData : ScriptableObject
    {
        public abstract ISupply CreateSupplyState();
    }
}