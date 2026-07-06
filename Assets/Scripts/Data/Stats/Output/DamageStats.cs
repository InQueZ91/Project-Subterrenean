using System;

namespace Data.Stats.Output
{
    /// <summary>
    /// The subset of combat stats that HitEffects actually care about.
    /// Shared between ProjectileStats, BeamStats
    /// </summary>
    [Serializable]
    public struct DamageStats
    {
        public float damage;
        public float knockback;
    }
}