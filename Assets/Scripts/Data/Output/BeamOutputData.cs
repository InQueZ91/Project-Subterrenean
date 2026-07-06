using Data.HitEffects;
using Data.Stats.Output;
using Runtime;
using Runtime.Handlers;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Output
{
    /// <summary>
    /// Instant bounced-beam output - the railgun.
    /// </summary>
    [CreateAssetMenu(fileName="New Beam Output", menuName="Game/Output/Beam Output")]
    public class BeamOutputData : OutputData
    {
        public BeamStats stats;
        public HitEffect[] hitEffects;
        
        public override void Fire(WeaponModHandler mods, Vector3 origin, Vector3 direction, GameObject owner)
        {
            var modifiedStats = mods.Resolve(stats);
            BeamSpawner.Instance.Spawn(hitEffects, modifiedStats, origin, direction, owner);
        }
    }
}