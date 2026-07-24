using System;
using Data.HitEffects;

namespace Data.Stats.Output
{
    [Serializable]
    public struct ProjectileStats
    {
        public float speed;
        public float damage;
        public float knockback;
        public float range;
        public int pierce;
        public int ricochet;
        public int spreadCount;
        public float spreadAngle;
        public bool useGravity;
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