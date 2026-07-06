using Data.Items;
using Data.Stats.Supplies;
using Runtime.Weapons.Supplies;
using UnityEngine;

namespace Data.Supplies
{
    /// <summary>
    /// Conventional gun ammo economy - a loaded buffer distinct from
    /// an external reserve, refilled via a timed reload.
    /// </summary>
    [CreateAssetMenu(fileName = "New Reloadable Supply", menuName = "Game/Supplies/Reloadable Supply")]
    public class ReloadableSupplyData : SupplyData
    {
        public AmmoType ammoType; // Key used to pull from the inventory reserve
        public AmmoStats stats;
        
        public override ISupply CreateSupplyState() => new ReloadableSupply(ammoType, stats);
    }
}