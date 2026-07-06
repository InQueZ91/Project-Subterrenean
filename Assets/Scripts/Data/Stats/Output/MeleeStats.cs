using System;

namespace Data.Stats.Output
{
    [Serializable]
    public struct MeleeStats
    {
        public float damage;
        public float knockback;
        public float range;
        public float hitboxRadius;
        
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