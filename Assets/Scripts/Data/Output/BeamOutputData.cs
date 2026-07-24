using Data.Stats.Output;
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
        
        public override void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            BeamSpawner.Instance.Spawn(stats.hitEffects, stats, origin, direction, owner);
        }
    }
}