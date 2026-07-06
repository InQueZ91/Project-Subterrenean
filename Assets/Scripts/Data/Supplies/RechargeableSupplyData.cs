using Data.Stats.Supplies;
using Runtime.Weapons.Supplies;
using UnityEngine;

namespace Data.Supplies
{
    /// <summary>
    /// Self-charging ammo economy - no external reserve,
    /// regenerate passively over time with a cooldown after firing.
    /// </summary>
    [CreateAssetMenu(fileName = "New Rechargeable Supply", menuName = "Game/Supplies/Rechargeable Supply")]
    public class RechargeableSupplyData : SupplyData
    {
        public BatteryStats stats;
        public override ISupply CreateSupplyState() => new RechargeableSupply(stats);
    }
}