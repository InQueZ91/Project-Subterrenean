using Data.Stats;
using Data.Stats.Output;
using UnityEngine;

namespace Data.Mods
{
    [CreateAssetMenu(menuName = "Game/Mods/Damage Boost")]
    public class DamageBoost : WeaponMod, IModifies<ProjectileStats>
    { 
        public float multiplier = 2f;
        public void Apply(ref ProjectileStats stats)
        {
            stats.damage *= multiplier;
        }
    }
}