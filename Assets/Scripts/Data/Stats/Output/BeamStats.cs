using System;

namespace Data.Stats.Output
{
    [Serializable]
    public struct BeamStats
    {
        public float damage;
        public float knockback;
        public float range;
        public int maxBounces;

        public DamageStats ToDamageStats()
        {
            return new DamageStats
            {
                damage = damage,
                knockback = knockback
            };
        }
    }
}