using Data.Stats.Supplies;
using UnityEngine;

namespace Runtime.Weapons.Supplies
{
    public class RechargeableSupply : IRechargeableSupply
    {
        public BatteryStats Stats { get; }
        public float CurrentCharge { get; private set; }
        public float MaxCharge { get; }
        
        private float _rechargeAvailableAt; // time after which regen resumes

        public RechargeableSupply(BatteryStats stats)
        {
            Stats = stats;
            MaxCharge = Stats.maxCharge;
            _rechargeAvailableAt = Time.time + Stats.rechargeDelay;
        }

        public bool HasEnough()
        {
            return CurrentCharge >= Stats.firingCost;
        }

        public void Spend()
        {
            CurrentCharge = Mathf.Max(0, CurrentCharge - Stats.firingCost);
            _rechargeAvailableAt = Time.time + Stats.rechargeDelay;
        }

        public void Recharge(float deltaTime)
        {
            if (Time.time < _rechargeAvailableAt) return;
            if (CurrentCharge >= MaxCharge) return;

            CurrentCharge = Mathf.Min(CurrentCharge + Stats.chargeRate * deltaTime, MaxCharge);
        }
    }
}