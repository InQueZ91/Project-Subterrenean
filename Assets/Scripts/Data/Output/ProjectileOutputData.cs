using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Handlers;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Output
{
    /// <summary>
    /// A traveling projectile output - guns and thrown items like grenades.
    /// </summary>
    [CreateAssetMenu(fileName = "New Projectile", menuName = "Game/Output/Projectile Data")]
    public class ProjectileOutputData : OutputData
    {
        public GameObject prefab;
        public ProjectileStats stats;
        public HitEffect[] hitEffects;
        
        public override void Fire(WeaponModHandler mods, Vector3 origin, Vector3 direction, GameObject owner)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"ProjectileData '{name}': missing prefab.", this);
                return;
            }

            var modifiedStats = mods.Resolve(stats);
            var count = Mathf.Max(1, modifiedStats.spreadCount);

            for (var i = 0; i < count; i++)
            {
                var spreadDirection = GetSpreadDirection(direction, modifiedStats.spreadAngle);
                ProjectileSpawner.Instance.Spawn(this, modifiedStats, origin, spreadDirection, owner);
            }
        }

        private static Vector3 GetSpreadDirection(Vector3 baseDirection, float halfAngle)
        {
            if (halfAngle <= 0f) return baseDirection;

            var angle = Random.Range(0f, halfAngle);
            var rotation = Random.Range(0f, 360f);

            var perpendicular = Vector3.Cross(baseDirection, Vector3.up);
            if (perpendicular.sqrMagnitude < 0.001f)
                perpendicular = Vector3.Cross(baseDirection, Vector3.right);

            var coneRotation = Quaternion.AngleAxis(rotation, baseDirection) * Quaternion.AngleAxis(angle, perpendicular.normalized);
            return coneRotation * baseDirection;
        }
    }
}