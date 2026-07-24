using System;
using Data.HitEffects;

namespace Data.Stats.Output
{
    [Serializable]
    public struct BeamStats
    {
        public float damage;
        public float knockback;
        public float range;
        public int maxBounces;
        public HitEffect[] hitEffects;

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