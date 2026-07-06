using System;

namespace Data.Stats.Supplies
{
    [Serializable]
    public struct BatteryStats
    {
        public float maxCharge;
        public float chargeRate;
        public float firingCost;
        public float rechargeDelay; // Seconds after firing before regen resumes
    }
}