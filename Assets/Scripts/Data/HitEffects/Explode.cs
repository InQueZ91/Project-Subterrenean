using Runtime;
using UnityEngine;

namespace Data.HitEffects
{
    [CreateAssetMenu(fileName = "New Explode Effect", menuName = "Game/Hit Effects/Explode")]
    public class Explode : HitEffect
    {
        [Header("Properties")]
        public float radius = 2f;
        [Tooltip("How quickly the explosion's effect diminishes with distance")]
        public float falloff = 0.5f;
        
        [Header("Collision")]
        [Tooltip("Which layers to check for collisions")]
        public LayerMask hitLayers;
        
        // Pre-allocated buffer at the class level — no allocation per shot
        private readonly Collider[] _overlapBuffer = new Collider[32];

        public override void Apply(HitContext ctx)
        {
            PlayHitEffectVFX(ctx);
            
            var hitCount = Physics.OverlapSphereNonAlloc(ctx.point, radius, _overlapBuffer, hitLayers);

            for (var i = 0; i < hitCount; i++)
            {
                var col = _overlapBuffer[i];
                
                if (!isFriendlyFire && col.gameObject == ctx.owner) continue;

                var damageable = col.GetComponent<IDamageable>();
                if (damageable == null) continue;

                var victimCenter = col.bounds.center;
                var toVictim = victimCenter - ctx.point;
                var distance = toVictim.magnitude;

                var normalizedDist = Mathf.Clamp01(distance / radius);
                var damageMultiplier = Mathf.Pow(1f - normalizedDist, falloff);

                var knockbackDir = toVictim.normalized;

                damageable.TakeDamage(
                    ctx.stats.damage * damageMultiplier,
                    knockbackDir * ctx.stats.knockback * damageMultiplier
                );
            }
        }

        private void PlayHitEffectVFX(HitContext ctx)
        {
            if (hitVFX == null) return;
            Instantiate(hitVFX, ctx.point, Quaternion.identity);
        }
    }
}