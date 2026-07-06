using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Handlers;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Output
{
    /// <summary>
    /// Instant close-range hit-test output
    /// No travel, no ammo
    /// pairing required (melee weapon typically uses InfiniteAmmoData)
    /// </summary>
    [CreateAssetMenu(fileName = "New Melee Output", menuName = "Game/Output/Melee Output")]
    public class MeleeOutputData : OutputData
    {
        public MeleeStats stats;
        public HitEffect[] hitEffects;
        
        public override void Fire(WeaponModHandler mods, Vector3 origin, Vector3 direction, GameObject owner)
        {
            var modifiedStats = mods.Resolve(stats);
            MeleeSpawner.Instance.Spawn(hitEffects, modifiedStats, origin, direction, owner);
        }
    }
}