using Runtime;
using UnityEngine;

namespace Data.HitEffects
{
    [CreateAssetMenu(fileName = "New Direct Hit Effect", menuName = "Game/Hit Effects/Direct Hit")]
    public class DirectHit : HitEffect
    {
        public override void Apply(HitContext ctx)
        {
            if (!isFriendlyFire && ctx.victim == ctx.owner) return;
            
            var damageable = ctx.victim.GetComponent<IDamageable>();
            if (damageable == null) return;
    
            var knockbackDir = ctx.direction.normalized;
            damageable.TakeDamage(ctx.stats.damage, knockbackDir * ctx.stats.knockback);
        }
    }
}