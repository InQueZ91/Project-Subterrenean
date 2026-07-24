using Data.Output;
using Data.Stats.Output;
using Runtime.Spawners;
using UnityEngine;

namespace Data.HitEffects
{
    [CreateAssetMenu(fileName = "New Cluster Effect", menuName = "Game/Hit Effects/Cluster")]
    public class Cluster : HitEffect
    {
        [Header("Sub Projectiles")]
        [SerializeField] [Range(0,1)] private float falloff;
        [SerializeField] private int count = 6;
        [SerializeField] private float spreadAngle = 45f;

        [Header("Sub Projectile Stats")]
        [SerializeField] private ProjectileOutputData subProjectileOutput;

        public override void Apply(HitContext ctx)
        {
            for (var i = 0; i < count; i++)
            {
                var azimuth = (360f / count) * i;
                var dir = Quaternion.Euler(-Mathf.Abs(spreadAngle), azimuth, 0) * Vector3.forward;
                
                var subStats = subProjectileOutput.stats;
                subStats.damage = ctx.stats.damage * falloff;
                subStats.knockback = ctx.stats.knockback * falloff;
                
                ProjectileSpawner.Instance.Spawn(
                    subProjectileOutput,
                    subStats,
                    ctx.point,
                    dir,
                    ctx.owner
                );
            }
        }
    }
}